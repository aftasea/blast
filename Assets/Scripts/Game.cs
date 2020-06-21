using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Game
{
	public enum State
	{
		WaitingForInput,
		ProcessingInput,
		ClearingMatches,
		TilesFalling
	}

	public static State CurrentState
	{
		get;
		set;
	} = State.WaitingForInput;
}
