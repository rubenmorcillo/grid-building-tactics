using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CombatData
{
	public int movementRange;
	public bool movementDone;
	public bool actionDone;

	public CombatData()
	{
		if (movementRange <= 0) movementRange = 5;
		movementDone = false;
		actionDone = false;
	}
}
