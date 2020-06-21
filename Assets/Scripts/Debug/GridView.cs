using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class GridView : MonoBehaviour
{
	public TileDefinition tileColors;

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
				AppendColorIndex(grid[r, c]);					
			}
			sb.AppendLine();
		}

		output.text = sb.ToString();
	}

	private void AppendColorIndex(int type)
	{
		if (type >= 0)
		{
			sb.Append("<color=#");
			sb.Append(ColorUtility.ToHtmlStringRGB(tileColors.colors[type]));
			sb.Append(">");
			sb.Append(type);
			sb.Append("</color> ");
		}
		else
		{
			sb.Append("_ ");
		}
	}
}
