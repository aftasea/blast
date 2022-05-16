﻿using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
	[SerializeField]
	private LevelDefinition level = null;
	[SerializeField]
	private Tile tilePrefab = null;

	[SerializeField]
	private int minTilesToMatch = 2;


	public LevelDefinition Level => level;

	private const int emptyCell = -1;
	private const int notFound = -1;
	
	private InputHandler inputHandler;

	private int[,] grid;
	private Tile[,] tiles;
	private bool[,] checkedTiles;
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
		checkedTiles = new bool[rows, columns];

		Transform myTransform = transform;

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				grid[r, c] = GetRandomBlockIndex();
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
		t.Init(grid[row, column], gridPosition);
		tiles[row, column] = t;
	}

	private int GetRandomBlockIndex()
	{
		return Random.Range(0, level.numberOfBlocks);
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

	private static void WaitForInput()
	{
		Game.CurrentState = Game.State.WaitingForInput;
	}

	private void CheckMatchFrom(int row, int column, int tileType)
	{
		if (HasTileAlreadyBeenChecked(row, column))
			return;

		// not the color I'm looking for
		if (grid[row, column] != tileType)
			return;

		checkedTiles[row, column] = true;
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

	private bool HasTileAlreadyBeenChecked(int row, int column) {
		return checkedTiles[row, column];
	}

	private void ClearMatches()
	{
		Game.CurrentState = Game.State.ClearingMatches;

		for (int i = matches.Count - 1; i >= 0; i--)
		{
			GridPosition pos = matches[i];
			grid[pos.row, pos.col] = emptyCell;
			tiles[pos.row, pos.col].Clear(OnTileCleared);
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
			CountEmptyTilesInColumn(c);
			DropExistingTilesInColumn(c);
			DropNewTilesInColumn(c);
		}
	}

	private void CountEmptyTilesInColumn(int c)
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
		// find first non empty tile above the given one
		for (int rAbove = r - 1; rAbove >= 0; rAbove--)
		{
			if (grid[rAbove, c] != emptyCell)
				return rAbove;
		}

		return notFound;
	}

	private void DropTile(int r, int c, int rAbove)
	{
		tiles[r, c].UpdateSprite(grid[rAbove, c]);

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

	private void DropNewTile(int c, int tileNumber)
	{
		int destinationRow = newTileCount - (tileNumber + 1);
		int originRow = -(tileNumber + 1);

		grid[destinationRow, c] = GetRandomBlockIndex();
		Tile newTile = tiles[destinationRow, c];
		newTile.UpdateSprite(grid[destinationRow, c]);
		newTile.StartFallingFrom(originRow, destinationRow, UpdateLanded);

		tileFallingCount++;
	}
}
