using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

	private GridXZ<PathNode> grid;
	public int x;
	public int z;
	public float worldX;
	public float worldZ;

	public int gCost;
	public int hCost;
	public int fCost;
	public bool visited;


	public bool isWalkable;
	public bool current = false;
	public bool selectable = false;
	public bool target = false;

	public PathNode cameFromNode;

	public int distance;


	//La sugerencia de chatGPT para precargar los vecinos
	//public PathNode[] Neighbours { get; private set; }
	public List<PathNode> neighbours = new List<PathNode>();

	public List<Directions.Cardinal> exterior = new List<Directions.Cardinal>();


	public PathNode(GridXZ<PathNode> grid, int x, int z)
	{
		this.grid = grid;
		this.x = x;
		this.z = z;

		visited = false;
		isWalkable = true;

		// Pre-calculate neighbours
		//Neighbours = new PathNode[8];
		//for (int i = 0; i < 8; i++)
		//{
		//	int checkX = x + PathfindingUtils.GetDeltaX(i);
		//	int checkZ = z + PathfindingUtils.GetDeltaZ(i);
		//	if (grid.IsValidPosition(checkX, checkZ))
		//	{
		//		Neighbours[i] = grid.GetGridObject(checkX, checkZ);
		//	}
		//
		//
	}

	public void SetIsWalkable(bool isWalkable)
	{
		this.isWalkable = isWalkable; 
	}
	
	public void CalculateFcost()
	{
		fCost = gCost + hCost;
	}


	public void Reset()
	{
		//coveragesDirections.Clear();
		//coveragesValidDirections.Clear();
		exterior.Clear();
		current = false;
		target = false;
		selectable = false;
		//offeringCoverage = false;

		visited = false;
		//cameFromNode = null; 
		distance = 0;

		//f = g = h = 0;
	}

	public override string ToString()
	{
		return x + "," + z;
	}

	public string ToStringFull()
	{
		return x + "," + z
		+ "\nCURRENT: " + current
			+ "\nTarget: " + target
			+ "\nWalkable: " + isWalkable
			+ "\nselectable: " + selectable
			+ "\ndistance: " + distance
			+ "\ngCost: " + gCost
			+ "\nhCost: " + hCost
			+ "\nfCost: " + fCost;
	}



}
