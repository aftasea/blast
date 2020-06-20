using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField]
	private LevelDefinition level;
	[SerializeField]
	private TileColors tileColors;
	[SerializeField]
	private Tile tilePrefab;

	private Tile[,] tiles;

	private int[,] grid;
	public int[,] Grid
	{
		get { return grid; }
	}

	private void Awake()
	{
		Initialise(level.rows, level.columns);
	}

	private void Initialise(int rows, int columns)
	{
		grid = new int[rows, columns];
		tiles = new Tile[rows, columns];

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

	private void CreateTile(int row, int column, Transform parentTransform)
	{
		Vector3 pos = new Vector3(column, -row, 0);
		Tile t = Instantiate(tilePrefab, pos, Quaternion.identity, parentTransform);
		t.SetColor(tileColors.colors[grid[row, column]]);
		tiles[row, column] = t;
	}

	private int GetRandomTileIndex()
	{
		return Random.Range(0, level.numberOfColors);
	}
}
