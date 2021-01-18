using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TileColors")]
public class TileDefinition : ScriptableObject
{
	public Color[] colors = new Color[6];
	public Sprite[] sprites = new Sprite[6];

	[Space]

	public Color clearColor;
	public Color invalidMoveColor;

	[Space]

	[Tooltip("Units per second")]
	public float fallingSpeed = 4f;

	[Tooltip("In seconds")]
	public float clearAnimationTime = 0.2f;

	[Tooltip("In seconds")]
	public float invalidMoveAnimationTime = 0.2f;
}
