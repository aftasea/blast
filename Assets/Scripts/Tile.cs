using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Tile : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Units per second")]
	private float fallSpeed = 4f;

	private SpriteRenderer spriteRenderer;

	private GridPosition gridPos;
	private Color tileColor;

	private System.Action<GridPosition /*origin*/, GridPosition /*destination*/> OnLandedCallback;
	private Transform myTransform;
	private bool isFalling;
	public bool Landed { get; private set; } = true;

	private GridPosition destination;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		myTransform = transform;
	}

	private void Update()
	{
		if (isFalling)
		{
			UpdateFall();
		}
	}

	private void UpdateFall()
	{
		Vector3 pos = myTransform.position;
		float posToRow = -pos.y;
		if (posToRow < destination.row)
		{
			pos.y -= fallSpeed * Time.deltaTime;
		}
		else // stop falling
		{
			OnLandedCallback?.Invoke(gridPos, destination);
			OnLandedCallback = null;
			isFalling = false;
			pos.y = -gridPos.row;
		}
		
		myTransform.position = pos;
	}

	public void UpdatePosition(GridPosition pos)
	{
		gridPos = pos;
		UpdateName(pos);
	}

	public void UpdateName(GridPosition pos)
	{
		StringBuilder sb = new StringBuilder("Tile_");
		sb.Append(pos.row);
		sb.Append('_');
		sb.Append(pos.col);

		name = sb.ToString();
	}

	public void MarkAsLanded()
	{
		Landed = true;
	}

	public void SetColor(Color c)
	{
		spriteRenderer.color = c;
		tileColor = c;
	}

	public void TemporaryTint(Color c, float time)
	{
		spriteRenderer.color = c;
		StartCoroutine(RestoreColor(time));
	}

	private IEnumerator RestoreColor(float time)
	{
		yield return new WaitForSeconds(time);

		spriteRenderer.color = tileColor;
	}

	public void Clear()
	{
		MarkAsEmpty();
	}

	public void MarkAsEmpty()
	{
		SetColor(new Color(0, 1, 1, 0.15f));
	}

	public void StartFall(GridPosition destination, System.Action<GridPosition, GridPosition> onLandedCallback)
	{
		this.destination = destination;
		isFalling = true;
		Landed = false;

		if (OnLandedCallback != null)
			Debug.LogWarning("OnLandedCallback already assigned");
		OnLandedCallback = onLandedCallback;
	}

	public void StartFallingFrom(
		GridPosition origin,
		GridPosition destination,
		System.Action<GridPosition, GridPosition> onLandedCallback)
	{
		Vector3 pos = myTransform.position;
		pos.y = origin.row;
		myTransform.position = pos;

		StartFall(destination, onLandedCallback);
	}
}
