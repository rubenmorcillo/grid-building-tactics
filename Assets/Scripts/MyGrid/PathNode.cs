using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

	private GridXZ<PathNode> grid;
	public int x;
	public int z;

	public int gCost;
	public int hCost;
	public int fCost;

	public bool isWalkable;
	public bool current = false;
	public bool selectable = false;


	public PathNode cameFromNode;

	//La sugerenciad e CHAPGPT para precargar los vecinos
	//public PathNode[] Neighbours { get; private set; }


	public PathNode(GridXZ<PathNode> grid, int x, int z)
	{
		this.grid = grid;
		this.x = x;
		this.z = z;

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
	public override string ToString()
	{
		return x + "," + z;
	}



}
