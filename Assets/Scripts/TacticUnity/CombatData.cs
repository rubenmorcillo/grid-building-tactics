using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CombatData
{
	public int maxMovementPoints;
	public int currentMovementPoints;
	public bool accionRealizada;

	public CombatData()
	{
		if (maxMovementPoints <= 0) maxMovementPoints = 5;
		currentMovementPoints = maxMovementPoints;
		accionRealizada = false;
	}
}
