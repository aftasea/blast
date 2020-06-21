using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Tile : MonoBehaviour
{
	[SerializeField]
	private TileDefinition config = null;

	private SpriteRenderer spriteRenderer;

	private GridPosition gridPos;
	private Color tileColor;

	private System.Action OnEffectEndCallback;
	private System.Action OnLandedCallback;
	private Transform myTransform;
	private bool isFalling;

	private int destinationRow;

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
		if (posToRow < destinationRow)
		{
			pos.y -= config.fallingSpeed * Time.deltaTime;
		}
		else // landed
		{
			OnLandedCallback?.Invoke();
			OnLandedCallback = null;
			isFalling = false;
			pos.y = -gridPos.row;
		}
		
		myTransform.position = pos;
	}

	public void UpdateColor(int colorIndex)
	{
		if (colorIndex < 0 || colorIndex >= config.colors.Length)
			colorIndex = 0;

		SetColor(config.colors[colorIndex]);
	}

	public void UpdatePosition(GridPosition pos)
	{
		gridPos = pos;
		UpdateName(pos);
	}

	private void UpdateName(GridPosition pos)
	{
		StringBuilder sb = new StringBuilder("Tile_");
		sb.Append(pos.row);
		sb.Append('_');
		sb.Append(pos.col);

		name = sb.ToString();
	}

	public void SetColor(Color c)
	{
		spriteRenderer.color = c;
		tileColor = c;
	}

	public void ShowInvalidMove(System.Action onEffectEndCallback)
	{
		OnEffectEndCallback = onEffectEndCallback;
		TemporaryTint(Color.gray, config.invalidMoveAnimationTime);
	}

	public void Clear(System.Action onClearedCallback)
	{
		OnEffectEndCallback = onClearedCallback;
		TemporaryTint(Color.cyan, config.clearAnimationTime);
	}

	private void TemporaryTint(Color c, float time)
	{
		spriteRenderer.color = c;
		StartCoroutine(RestoreColor(time));
	}

	private IEnumerator RestoreColor(float time)
	{
		yield return new WaitForSeconds(time);

		spriteRenderer.color = tileColor;

		OnEffectEndCallback?.Invoke();
		OnEffectEndCallback = null;
	}

	public void StartFallingFrom(int originRow, int destinationRow,	System.Action onLandedCallback)
	{
		Vector3 pos = myTransform.position;
		pos.y = -originRow;
		myTransform.position = pos;

		this.destinationRow = destinationRow;
		isFalling = true;

		if (OnLandedCallback != null)
			Debug.LogWarning("OnLandedCallback already assigned");
		OnLandedCallback = onLandedCallback;
	}
}
