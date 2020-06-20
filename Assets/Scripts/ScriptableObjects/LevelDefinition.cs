using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelDefinition", order = 1)]
public class LevelDefinition : ScriptableObject
{
	[Range(5, 20)]
	public int rows = 5;

	[Range(5, 20)]
	public int columns = 5;
}
