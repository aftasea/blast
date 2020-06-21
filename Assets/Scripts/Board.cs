using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField]
	private LevelDefinition level = null;
	[SerializeField]
	private TileColors tileColors = null;
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
		if (pos.row < level.rows && pos.col < level.columns)
			DetectMatches(pos.row, pos.col);
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
		System.Array.Clear(checkedTiles, 0, checkedTiles.Length);
		matches.Clear();

		int tileType = grid[row, column];
		CheckMatchFrom(row, column, tileType);

		if (matches.Count >= minTilesToMatch)
			ClearMatches();
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
		for (int i = matches.Count - 1; i >=0; i--)
		{
			grid[matches[i].row, matches[i].col] = emptyCell;
			tiles[matches[i].row, matches[i].col].Clear();
		}

		HandleEmptyTiles();
	}

	private void HandleEmptyTiles()
	{
		for (int c = level.columns - 1; c >= 0; c--)
		{
			CollectEmptyTilesInColumn(c);

			// drop tiles
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
		tiles[rAbove, c].MarkAsEmpty();

		grid[r, c] = grid[rAbove, c];
		grid[rAbove, c] = emptyCell;
		
		tiles[r, c].StartFallingFrom(
			new GridPosition(rAbove, c),
			new GridPosition(r, c),
			UpdateLanded
		);
	}

	private void UpdateLanded(GridPosition origin, GridPosition destination)
	{
		//TODO: handle block user input until all animations end
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
		int destRow = newTileCount - (tileIndex + 1);

		grid[destRow, c] = GetRandomTileIndex();
		tiles[destRow, c].SetColor(GetColor(new GridPosition(destRow, c)));
		tiles[destRow, c].StartFallingFrom(
			new GridPosition(-(tileIndex + 1), c),
			new GridPosition(destRow, c),
			UpdateLanded
		);
	}
}
