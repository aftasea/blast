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

	private const int emptyCell = -1;

	private Tile[,] tiles;
	private int[,] checkedTiles;
	private List<GridPosition> matches = new List<GridPosition>();


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
		t.UpdateName(row, column);
		t.SetColor(tileColors.colors[grid[row, column]]);
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
			tiles[matches[i].row, matches[i].col].Clear();
			grid[matches[i].row, matches[i].col] = emptyCell;
		}

		StartCoroutine( CheckTilesToFall() );
	}

	private IEnumerator CheckTilesToFall()
	{
		for (int c = level.columns - 1; c >= 0; c--)
		{
			for (int r = level.rows - 1; r >= 0; r--)
			{
				if (grid[r, c] == emptyCell)
				{
					tiles[r, c].MarkAsEmpty();

					yield return new WaitForSeconds(0.1f);

					yield return DropTileAbove(r, c);
				}
			}
		}
	}

	private IEnumerator DropTileAbove(int r, int c)
	{
		// get first non empty tile above this one
		for (int rAbove = r; rAbove >= 0; rAbove--)
		{
			if (grid[rAbove, c] != emptyCell)
			{
				grid[r, c] = grid[rAbove, c];
				grid[rAbove, c] = emptyCell;
				tiles[r, c].SetColor(tileColors.colors[grid[r, c]]);
				tiles[rAbove, c].MarkAsEmpty();
				yield return new WaitForSeconds(0.2f);
				yield break;
			}
		}

	}
}
