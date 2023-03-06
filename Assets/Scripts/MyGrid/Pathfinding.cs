using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

	private const int MOVE_STRAIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 12;
	private GridXZ<PathNode> grid;
	private List<PathNode> openList;
	private List<PathNode> closedList;

	public static Pathfinding Instance { get; private set; }
	public Pathfinding(int width, int height, float cellSize)
	{
		Instance = this;
		grid = new GridXZ<PathNode>(width, height, cellSize, Vector3.zero, (GridXZ<PathNode> g, int x, int y) => new PathNode(g,x,y));
	}

	public GridXZ<PathNode> GetGrid()
	{
		return grid;
	}

	public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
	{
		grid.GetXZ(startWorldPosition, out int startX, out int startZ);
		grid.GetXZ(endWorldPosition, out int endX, out int endZ);

		List<PathNode> path = FindPath(startX, startZ, endX, endZ);
		if (path == null)
		{
			return null;
		}
		else
		{
			List<Vector3> vectorPath = new List<Vector3>();
			foreach(PathNode pathNode in path)
			{
				vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.z) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * 0.5f);
			}
			return vectorPath;
		}

	}
	public  List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
	{
		PathNode startNode = grid.GetGridObject(startX, startZ);
		PathNode endNode = grid.GetGridObject(endX, endZ);
		openList = new List<PathNode> { startNode };
		closedList = new List<PathNode>();

		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int z = 0; z < grid.GetHeight(); z++)
			{
				PathNode pathNode = grid.GetGridObject(x, z);
				pathNode.gCost = int.MaxValue;
				pathNode.CalculateFcost();
				pathNode.cameFromNode = null;
			}
		}

		startNode.gCost = 0;
		startNode.hCost = CalculateDistanceCost(startNode, endNode);
		startNode.CalculateFcost();

		while (openList.Count > 0)
		{
			PathNode currentNode = GetLowestFCostNode(openList);
			if (currentNode == endNode)
			{
				//alcanzada la última casilla del camino
				return CalculatePath(endNode);
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
			{
				if (closedList.Contains(neighbourNode)) continue;
				if (!neighbourNode.walkable)
				{
					closedList.Add(neighbourNode);
					continue;
				}

				int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
				if (tentativeGCost < neighbourNode.gCost)
				{
					neighbourNode.cameFromNode = currentNode;
					neighbourNode.gCost = tentativeGCost;
					neighbourNode.gCost = CalculateDistanceCost(neighbourNode, endNode);
					neighbourNode.CalculateFcost();

					if (!openList.Contains(neighbourNode))
					{
						openList.Add(neighbourNode);
					}

				}
			}
		}

		//out of nodes on the openList
		return null;
	}

	private List<PathNode> GetNeighbourList(PathNode currentNode)
	{
		List<PathNode> neighbourList = new List<PathNode>();

		if (currentNode.x  - 1 >= 0)
		{
			//izquierda
			neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z));

			//izquierda abajo
			if (currentNode.z - 1 >= 0)
			{
				neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
			}
			//izquierda arriba
			if (currentNode.z + 1 < grid.GetHeight())
			{
				neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
			}
		}
		if (currentNode.x + 1 < grid.GetWidth())
		{
			//Derecha
			neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z));
			//derecha abajo
			if (currentNode.z - 1 >= 0)
			{
				neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
			}
			//derecha arriba
			if (currentNode.z + 1 < grid.GetHeight())
			{
				neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
			}
		}
		//abajo
		if (currentNode.z - 1 >= 0)
		{
			neighbourList.Add(GetNode(currentNode.x, currentNode.z - 1));

		}
		//arriba
		if (currentNode.z + 1 < grid.GetHeight())
		{
			neighbourList.Add(GetNode(currentNode.x, currentNode.z + 1));
		}

		return neighbourList;
	}

	private PathNode GetNode(int x, int z)
	{
		return grid.GetGridObject(x, z);
	}

	private List<PathNode> CalculatePath(PathNode endNode)
	{
		List<PathNode> path = new List<PathNode>();
		path.Add(endNode);
		PathNode currentNode = endNode;
		while(currentNode.cameFromNode != null)
		{
			path.Add(currentNode.cameFromNode);
			currentNode = currentNode.cameFromNode;
		}
		path.Reverse();
		return path;
	}

	private int CalculateDistanceCost(PathNode a, PathNode b)
	{
		int xDistance = Mathf.Abs(a.x - b.x);
		int zDistance = Mathf.Abs(a.z - b.z);
		int remaining = Mathf.Abs(xDistance - zDistance);
		return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
	}

	private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
	{
		PathNode lowerFCostNode = pathNodeList[0];
		for (int i = 1; i< pathNodeList.Count; i++)
		{
			if (pathNodeList[i].fCost < lowerFCostNode.fCost)
			{
				lowerFCostNode = pathNodeList[i];
			}
		}
		return lowerFCostNode;
	}
}
