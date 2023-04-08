using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions 
{
    public enum Cardinal {ESTE , OESTE, NORTE, SUR}
    public static Vector3 GetVectorFromCardinal(Cardinal cardinal)
	{
		Vector3 direction = new Vector3();
		switch (cardinal)
		{
			case Cardinal.ESTE:
				direction = Vector3.right;
				break;
			case Cardinal.OESTE:
				direction = -Vector3.right;
				break;
			case Cardinal.NORTE:
				direction = Vector3.forward;
				break;
			case Cardinal.SUR:
				direction = -Vector3.forward;
				break;

		}

		return direction;
	}
}
