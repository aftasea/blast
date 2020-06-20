using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField]
	private LevelDefinition level;

	private int[,] grid;
	public int[,] Grid
	{
		get { return grid; }
	}

	private void Awake()
	{
		Initialise(level.rows, level.columns);
	}

	public void Initialise(int rows, int columns)
	{
		grid = new int[rows, columns];

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				grid[r, c] = 2;
			}
		}
	}
}
