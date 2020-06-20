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

	private InputHandler inputHandler;

	private Tile[,] tiles;

	private int[,] grid;
	public int[,] Grid
	{
		get { return grid; }
	}

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

	private void ProcessTap(int row, int column)
	{
		Debug.Log(row + ", " + column);
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
}
