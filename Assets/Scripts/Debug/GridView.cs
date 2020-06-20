using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class GridView : MonoBehaviour
{
	private Text output;
	private int[,] grid;

	private int rows;
	private int columns;

	private StringBuilder sb = new StringBuilder();

	private void Awake()
	{
		output = GetComponent<Text>();

	}

	private void Start()
	{
		Board board = FindObjectOfType<Board>();
		grid = board.Grid;
		rows = grid.GetLength(0);
		columns = grid.GetLength(1);
	}

	private void Update()
	{
		sb.Clear();

		for (int r = 0; r < rows; r++)
		{
			for (int c = 0; c < columns; c++)
			{
				sb.Append(grid[r, c]);
				sb.Append(" ");
			}
			sb.AppendLine();
		}

		output.text = sb.ToString();
	}
}
