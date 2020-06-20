using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Tile : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void UpdateName(int row, int column)
	{
		StringBuilder sb = new StringBuilder("Tile_");
		sb.Append(row);
		sb.Append('_');
		sb.Append(column);

		name = sb.ToString();
	}

	public void SetColor(Color c)
	{
		spriteRenderer.color = c;
	}
}
