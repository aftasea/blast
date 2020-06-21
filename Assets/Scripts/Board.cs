using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField]
	private LevelDefinition level = null;
	[SerializeField]
	private TileDefinition tileColors = null;
	[SerializeField]
	private Tile tilePrefab = null;

	[SerializeField]
	private int minTilesToMatch = 2;

	private InputHandler inputHandler;

	private int[,] grid;
	public int[,] Grid
	{
		get { return grid; }
	}

	public LevelDefinition Level
	{
		get { return level; }
	}

	private const int emptyCell = -1;
	private const int notFound = -1;

	private Tile[,] tiles;
	private int[,] checkedTiles;
	private List<GridPosition> matches = new List<GridPosition>();
	private int newTileCount;
	private int tileFallingCount;
	private int tileClearedCount;


	private void Awake()
	{
		Initialise(level.rows, level.columns);
		inputHandler = GetComponent<InputHandler>();
		inputHandler.OnTap += ProcessTap;
	}

	private void OnDestroy()
	{
		inputHandler.OnTap -= ProcessTap;
	}

	private void Initialise(int rows, int columns)
	{
		grid = new int[rows, columns];
		tiles = new Tile[rows, columns];
		checkedTiles = new int[rows, columns];

		Transform myTransform = transform;

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				grid[r, c] = GetRandomTileIndex();
				CreateTile(r, c, myTransform);
			}
		}
	}

	private void ProcessTap(GridPosition pos)
	{
		if (Game.CurrentState == Game.State.WaitingForInput)
		{
			if (pos.row < level.rows && pos.col < level.columns)
				DetectMatches(pos.row, pos.col);
		}
	}

	private void CreateTile(int row, int column, Transform parentTransform)
	{
		Vector3 pos = new Vector3(column, -row, 0);
		Tile t = Instantiate(tilePrefab, pos, Quaternion.identity, parentTransform);
		GridPosition gridPosition = new GridPosition(row, column);
		t.UpdatePosition(gridPosition);
		t.SetColor(GetColor(gridPosition));
		tiles[row, column] = t;
	}

	private int GetRandomTileIndex()
	{
		return Random.Range(0, level.numberOfColors);
	}

	private void DetectMatches(int row, int column)
	{
		Game.CurrentState = Game.State.ProcessingInput;

		System.Array.Clear(checkedTiles, 0, checkedTiles.Length);
		matches.Clear();

		int tileType = grid[row, column];
		CheckMatchFrom(row, column, tileType);

		if (matches.Count >= minTilesToMatch)
			ClearMatches();
		else
			tiles[row, column].ShowInvalidMove(WaitForInput);
	}

	private void WaitForInput()
	{
		Game.CurrentState = Game.State.WaitingForInput;
	}

	private Color GetColor(GridPosition pos)
	{
		int index = grid[pos.row, pos.col];
		if (index == emptyCell)
			return new Color(0, 0.4f, 1, 0.25f);
		return tileColors.colors[index];
	}

	private void CheckMatchFrom(int row, int column, int tileType)
	{
		// already checked
		if (checkedTiles[row, column] == 1)
			return;

		// not the color I'm looking for
		if (grid[row, column] != tileType)
			return;

		checkedTiles[row, column] = 1;
		matches.Add(new GridPosition(row, column));
		
		// Check left neighbour
		if (column - 1 >= 0)
			CheckMatchFrom(row, column - 1, tileType);

		// Check right neighbour
		if (column + 1 < level.columns)
			CheckMatchFrom(row, column + 1, tileType);
		
		// Check top neighbour
		if (row - 1 >= 0)
			CheckMatchFrom(row - 1, column, tileType);

		// Check bottom neighbour
		if (row + 1 < level.rows)
			CheckMatchFrom(row + 1, column, tileType);
	}

	private void ClearMatches()
	{
		Game.CurrentState = Game.State.ClearingMatches;

		for (int i = matches.Count - 1; i >=0; i--)
		{
			grid[matches[i].row, matches[i].col] = emptyCell;
			tiles[matches[i].row, matches[i].col].Clear(OnTileCleared);
		}
	}

	private void OnTileCleared()
	{
		++tileClearedCount;
		if (tileClearedCount >= matches.Count)
			HandleEmptyTiles();
	}

	private void HandleEmptyTiles()
	{
		Game.CurrentState = Game.State.TilesFalling;

		for (int c = level.columns - 1; c >= 0; c--)
		{
			CollectEmptyTilesInColumn(c);
			DropExistingTilesInColumn(c);
			DropNewTilesInColumn(c);
		}
	}

	private void CollectEmptyTilesInColumn(int c)
	{
		newTileCount = 0;

		for (int r = level.rows - 1; r >= 0; r--)
		{
			if (grid[r, c] == emptyCell)
				newTileCount++;
		}
	}

	private void DropExistingTilesInColumn(int c)
	{
		for (int r = level.rows - 1; r >= 0; r--)
		{
			if (grid[r, c] == emptyCell)
			{
				int rowAbove = RowOfTileAbove(r, c);
				if (rowAbove != notFound)
				{
					DropTile(r, c, rowAbove);
				}
			}
		}
	}

	private int RowOfTileAbove(int r, int c)
	{
		// find first non empty tile above this one
		for (int rAbove = r; rAbove >= 0; rAbove--)
		{
			if (grid[rAbove, c] != emptyCell)
				return rAbove;
		}

		return notFound;
	}

	private void DropTile(int r, int c, int rAbove)
	{
		tiles[r, c].SetColor(GetColor(new GridPosition(rAbove, c)));

		grid[r, c] = grid[rAbove, c];
		grid[rAbove, c] = emptyCell;
		
		tiles[r, c].StartFallingFrom(rAbove, r, UpdateLanded);

		tileFallingCount++;
	}

	private void UpdateLanded()
	{
		--tileFallingCount;
		if (tileFallingCount <= 0)
			Game.CurrentState = Game.State.WaitingForInput;
	}

	private void DropNewTilesInColumn(int c)
	{
		for (int i = newTileCount - 1; i >= 0; i--)
		{
			DropNewTile(c, i);
		}
	}

	private void DropNewTile(int c, int tileIndex)
	{
		int destinationRow = newTileCount - (tileIndex + 1);
		int originRow = -(tileIndex + 1);

		grid[destinationRow, c] = GetRandomTileIndex();
		tiles[destinationRow, c].SetColor(GetColor(new GridPosition(destinationRow, c)));
		tiles[destinationRow, c].StartFallingFrom(
			originRow,
			destinationRow,
			UpdateLanded
		);

		tileFallingCount++;
	}
}
