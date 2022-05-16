using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	[SerializeField]
	private Camera cam = null;
	
	public event Action<GridPosition> OnTap;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
			int row = Mathf.FloorToInt(-mouseWorldPos.y);
			int col = Mathf.FloorToInt(mouseWorldPos.x);

			if (row >= 0 && col >= 0)
				OnTap?.Invoke(new GridPosition(row, col));
		}
	}
}
