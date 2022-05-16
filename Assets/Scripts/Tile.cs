using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField]
	private BlockDefinition config = null;

	private SpriteRenderer spriteRenderer;

	private System.Action OnEffectEndCallback;
	private System.Action OnLandedCallback;
	private Transform myTransform;
	private bool isFalling;

	private int destinationRowPosition;

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
		if (pos.y > destinationRowPosition)
		{
			float nextPos = pos.y - (config.fallingSpeed * Time.deltaTime);
			pos.y = Mathf.Max(nextPos, destinationRowPosition);
		}
		else // landed
		{
			OnLandedCallback?.Invoke();
			OnLandedCallback = null;
			isFalling = false;
			pos.y = destinationRowPosition;
		}
		
		myTransform.position = pos;
	}

	public void Init(int spriteIndex, GridPosition pos)
	{
		UpdateSprite(spriteIndex);
		UpdateName(pos);
	}

	public void UpdateSprite(int spriteIndex)
	{
		if (spriteIndex < 0 || spriteIndex >= config.sprites.Length)
			spriteIndex = 0;

		SetSprite(config.sprites[spriteIndex]);
	}

	private void SetSprite(Sprite s)
	{
		spriteRenderer.sprite = s;
	}

	private void UpdateName(GridPosition pos)
	{
#if UNITY_EDITOR
		name = $"Tile_{pos.row}_{pos.col}";
#endif
	}

	public void ShowInvalidMove(System.Action onEffectEndCallback)
	{
		OnEffectEndCallback = onEffectEndCallback;
		TemporaryTint(config.invalidMoveColor, config.invalidMoveAnimationTime);
	}

	public void Clear(System.Action onClearedCallback)
	{
		OnEffectEndCallback = onClearedCallback;
		TemporaryTint(config.clearColor, config.clearAnimationTime);
	}

	private void TemporaryTint(Color c, float time)
	{
		spriteRenderer.color = c;
		StartCoroutine(RestoreColor(time));
	}

	private IEnumerator RestoreColor(float time)
	{
		yield return new WaitForSeconds(time);

		spriteRenderer.color = Color.white;

		OnEffectEndCallback?.Invoke();
		OnEffectEndCallback = null;
	}

	public void StartFallingFrom(int originRow, int destinationRow,	System.Action onLandedCallback)
	{
		Vector3 pos = myTransform.position;
		pos.y = -originRow;
		myTransform.position = pos;

		destinationRowPosition = -destinationRow;
		isFalling = true;

		if (OnLandedCallback != null)
			Debug.LogWarning("OnLandedCallback already assigned");
		OnLandedCallback = onLandedCallback;
	}
}
