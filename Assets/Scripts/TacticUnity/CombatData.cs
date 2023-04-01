using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CombatData
{
	public int movementRange;
	public int jumpHeight;

	public bool movementDone;
	public bool actionDone;
	public bool isAlive;

	//stats
	public int initiative;

	public CombatData()
	{
		if (movementRange <= 0) movementRange = 8;
		if (jumpHeight <= 0) jumpHeight = 1;
		if (initiative <= 0) initiative = 3;


		movementDone = false;
		actionDone = false;
		isAlive = true;
	}

	public void RestorePoints()
	{
		movementDone = false;
		actionDone = false;
	}
}
