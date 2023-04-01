using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{

	private const int MOVE_STRAIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 14;
	private GridXZ<PathNode> grid;
	private List<PathNode> pathNodeList = new List<PathNode>();

	private List<PathNode> selectableNodes = new List<PathNode>();
	private List<PathNode> openList;
	private List<PathNode> closedList;

	public static Pathfinding Instance { get; private set; }
	public Pathfinding(int width, int height, float cellSize)
	{
		Instance = this;
		grid = new GridXZ<PathNode>(width, height, cellSize, Vector3.zero, (GridXZ<PathNode> g, int x, int y) => new PathNode(g,x,y));
		StartPathNodeList();
		UpdateObstacles(grid);
		
	}

	void StartPathNodeList()
	{
		for (int x = 0; x < grid.GetWidth(); x++)
		{
			for (int y = 0; y < grid.GetHeight(); y++)
			{
				PathNode node = GetNode(x, y);
				node.worldX = grid.GetWorldPosition(node.x, node.z).x;
				node.worldZ = grid.GetWorldPosition(node.x, node.z).z;
				pathNodeList.Add(node);
			}
		}
	}

	public void DebugDrawSelectables()
	{
		foreach(PathNode pathNode in pathNodeList)
		{
			if (pathNode.selectable)
			Debug.DrawRay(new Vector3(pathNode.worldX + 2.5f, 0, pathNode.worldZ + 2.5f), Vector3.up, Color.blue, 0.5f);
		}
	}

	public void UpdateObstacles(GridXZ<PathNode> grid)
	{
		float cellSize = grid.GetCellSize();
		
		foreach(PathNode pathNode in pathNodeList)
		{
			int x = pathNode.x;
			int z = pathNode.z;

			Vector3 worldPosition = grid.GetWorldPosition(x, z);
			Vector3 centroNodo = new Vector3(worldPosition.x + cellSize / 2, 0, worldPosition.z + cellSize / 2);
			Debug.Log("Soy la casilla " + pathNode.ToStringFull() +"mi posición real en el mundo es " + grid.GetWorldPosition(pathNode.x, pathNode.z));

			Collider[] hits = Physics.OverlapBox(centroNodo, new Vector3(cellSize / 2.5f, cellSize / 2.5f, cellSize / 2.5f), Quaternion.identity, LayerMask.GetMask("Obstacles"));

			if (hits?.Length > 0)
			{
				pathNode.SetIsWalkable(false);
			}

		}
		
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
				if (!neighbourNode.isWalkable)
				{
					closedList.Add(neighbourNode);
					continue;
				}

				int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
				if (tentativeGCost < neighbourNode.gCost)
				{
					neighbourNode.cameFromNode = currentNode;
					neighbourNode.gCost = tentativeGCost;
					neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
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
		PathNode posibleVecino;
		if (currentNode.x  - 1 >= 0)
		{
			//izquierda
			posibleVecino = GetNode(currentNode.x - 1, currentNode.z);
			if (posibleVecino.isWalkable)
			{
				neighbourList.Add(posibleVecino);
			}

			//izquierda abajo
			posibleVecino = GetNode(currentNode.x - 1, currentNode.z - 1);
			if (currentNode.z - 1 >= 0)
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}
			//izquierda arriba
			posibleVecino = GetNode(currentNode.x - 1, currentNode.z + 1);
			if (currentNode.z + 1 < grid.GetHeight())
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}
		}
		if (currentNode.x + 1 < grid.GetWidth())
		{
			//Derecha
			posibleVecino = GetNode(currentNode.x + 1, currentNode.z);
			if (posibleVecino.isWalkable)
			{
				neighbourList.Add(posibleVecino);
			}
			//derecha abajo
			posibleVecino = GetNode(currentNode.x + 1, currentNode.z - 1);
			if (currentNode.z - 1 >= 0)
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}
			//derecha arriba
			posibleVecino = GetNode(currentNode.x + 1, currentNode.z + 1);
			if (currentNode.z + 1 < grid.GetHeight())
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}
		}
		//abajo
		posibleVecino = GetNode(currentNode.x, currentNode.z - 1);
		if (currentNode.z - 1 >= 0)
		{
			if (posibleVecino.isWalkable)
			{
				neighbourList.Add(posibleVecino);
			}

		}
		//arriba
		posibleVecino = GetNode(currentNode.x, currentNode.z + 1);
		if (currentNode.z + 1 < grid.GetHeight())
		{
			if (posibleVecino.isWalkable)
			{
				neighbourList.Add(posibleVecino);
			}
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

	public PathNode GetCurrentNode()
	{
		TacticUnity currentUnity = CombateManager.instance.GetCurrentUnity();
		return GetNode(Mathf.FloorToInt(currentUnity.transform.position.x / grid.GetCellSize()), Mathf.FloorToInt(currentUnity.transform.position.z / grid.GetCellSize()));
		 
	}

	public void FindSelectableNodes()
	{
		selectableNodes.Clear();
		//esto a lo mejor solo es para el player principal, ya veremos
		PathNode currentNode = GetCurrentNode();
		
		Queue<PathNode> process = new Queue<PathNode>();

		process.Enqueue(currentNode);
		currentNode.visited = true;

		while(process.Count > 0)
		{
			Debug.Log("jamaica");
			PathNode node = process.Dequeue();
			selectableNodes.Add(node);
			node.selectable = true;

			if (node.distance < CombateManager.instance.GetCurrentUnity().combatData.movementRange)
			{
				foreach(PathNode casilla in GetNeighbourList(node))
				{
					Debug.Log(" soy el vecino " + casilla);
					if (!casilla.visited)
					{
						casilla.cameFromNode = node;
						casilla.visited = true;
						casilla.distance = 1 + node.distance;
						process.Enqueue(casilla);
					}
				}
			}
			
		}
	}
}
