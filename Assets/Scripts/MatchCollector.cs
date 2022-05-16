using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCollector
{
	private readonly int[,] grid;
	private bool[,] checkedTiles;
	private List<GridPosition> matches;
	private int rowCount;
	private int columnCount;

	public MatchCollector(int[,] grid) {
		this.grid = grid;
	}

	public void Init(int rows, int columns) {
		rowCount = rows;
		columnCount = columns;
		checkedTiles = new bool[rows, columns];
		matches = new List<GridPosition>();
	}

	public List<GridPosition> DetectMatches(int row, int column)
	{
		System.Array.Clear(checkedTiles, 0, checkedTiles.Length);
		matches.Clear();
		
		int tileType = grid[row, column];
		CheckMatchFrom(row, column, tileType);

		return matches;
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
		if (column + 1 < columnCount)
			CheckMatchFrom(row, column + 1, tileType);
		
		// Check top neighbour
		if (row - 1 >= 0)
			CheckMatchFrom(row - 1, column, tileType);

		// Check bottom neighbour
		if (row + 1 < rowCount)
			CheckMatchFrom(row + 1, column, tileType);
	}

	private bool HasTileAlreadyBeenChecked(int row, int column) {
		return checkedTiles[row, column];
	}
}
