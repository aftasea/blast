using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TileColors")]
public class TileColors : ScriptableObject
{
	public Color[] colors = new Color[6];
}
