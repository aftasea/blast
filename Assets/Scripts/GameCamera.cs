using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	[Tooltip("Vertical offset from grid in units")]
	[SerializeField]
	private int verticalOffset = 2;
	
	private Camera cam;
	private LevelDefinition level;

	private void Awake()
	{
		cam = GetComponent<Camera>();

		Board b = FindObjectOfType<Board>();
		level = b.Level;

		UpdateCamera();
	}

	private void UpdateCamera()
	{
		UpdateOrtographicSize();
		UpdatePosition();
	}

	private void UpdateOrtographicSize()
	{
		float gridAspectRatio = (float)level.columns / level.rows;

		if (gridAspectRatio <= cam.aspect)
		{
			cam.orthographicSize = (level.rows + verticalOffset) / 2f;
		}
		else
		{
			float offset = verticalOffset / gridAspectRatio;
			float ratio = cam.aspect / gridAspectRatio;
			cam.orthographicSize = ((level.rows + offset) / ratio) / 2f;
		}
	}

	private void UpdatePosition()
	{
		float posX = level.columns / 2f;
		float posY =  -level.rows / 2f;
		cam.transform.position = new Vector3(posX, posY, cam.transform.position.z);
	}
}
