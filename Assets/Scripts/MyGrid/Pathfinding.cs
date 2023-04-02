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
			//Debug.Log("Soy la casilla " + pathNode.ToStringFull() +"mi posición real en el mundo es " + grid.GetWorldPosition(pathNode.x, pathNode.z));

			Collider[] hits = Physics.OverlapBox(centroNodo, new Vector3(cellSize / 2.5f, cellSize / 2.5f, cellSize / 2.5f), Quaternion.identity, LayerMask.GetMask("Obstacles"));

			if (hits?.Length > 0)
			{
				pathNode.SetIsWalkable(false);
			}

		}
		
	}

	public Vector3[] pathToVectorArray(List<PathNode> pathNodeList)
	{
		List<Vector3> pathArray = new List<Vector3>();

		foreach (PathNode pathNode in pathNodeList)
		{
			float offsetY = 0.5f;

			Vector3 offset = new Vector3(grid.GetCellSize() / 2, offsetY, grid.GetCellSize() / 2);

			Vector3 pathPoint = new Vector3(pathNode.worldX , 0, pathNode.worldZ)  + offset;
			pathArray.Add(pathPoint);
			
		}
		return pathArray.ToArray();
	}

	public GridXZ<PathNode> GetGrid()
	{
		return grid;
	}

	public List<PathNode> GetSelectableNodes()
	{
		return selectableNodes;
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

	private List<PathNode> GetNeighbourList(PathNode node)
	{
		List<PathNode> neighbourList = new List<PathNode>();
		PathNode posibleVecino;
		if (node.x  - 1 >= 0)
		{
			//izquierda
			if (!CheckWall(node, -Vector3.right))
			{
				posibleVecino = GetNode(node.x - 1, node.z);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}


			//izquierda abajo
			if (!CheckWall(node, -Vector3.right + Vector3.back))
			{
				posibleVecino = GetNode(node.x - 1, node.z - 1);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}
				
				if (node.z - 1 >= 0)
				{
					if (posibleVecino.isWalkable)
					{
						neighbourList.Add(posibleVecino);
					}
				}
			}

			//izquierda arriba
			if (!CheckWall(node, -Vector3.right + -Vector3.back))
			{
				posibleVecino = GetNode(node.x - 1, node.z + 1);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}

				if (node.z + 1 < grid.GetHeight())
				{
					if (posibleVecino.isWalkable)
					{
						neighbourList.Add(posibleVecino);
					}
				}
			}
			
		}
		if (node.x + 1 < grid.GetWidth())
		{
			//Derecha
			if (!CheckWall(node, Vector3.right))
			{
				posibleVecino = GetNode(node.x + 1, node.z);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}

				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}

			//derecha abajo
			if (!CheckWall(node, Vector3.right + -Vector3.back))
			{
				posibleVecino = GetNode(node.x + 1, node.z - 1);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}
				if (node.z - 1 >= 0)
				{
					if (posibleVecino.isWalkable)
					{
						neighbourList.Add(posibleVecino);
					}
				}
			}

			//derecha arriba
			if (!CheckWall(node, Vector3.right + Vector3.back))
			{
				posibleVecino = GetNode(node.x + 1, node.z + 1);
				if (posibleVecino != null && posibleVecino.selectable)
				{
					node.posibleNeighbours.Add(posibleVecino);

				}

				if (node.z + 1 < grid.GetHeight())
				{
					if (posibleVecino.isWalkable)
					{
						neighbourList.Add(posibleVecino);
					}
				}
			}
			
		}
		//abajo
		if (!CheckWall(node, Vector3.back))
		{
			posibleVecino = GetNode(node.x, node.z - 1);
			if (posibleVecino != null && posibleVecino.selectable)
			{
				node.posibleNeighbours.Add(posibleVecino);

			}

			if (node.z - 1 >= 0)
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}

			}
		}


		//arriba
		if (!CheckWall(node, -Vector3.back))
		{
			posibleVecino = GetNode(node.x, node.z + 1);
			if (posibleVecino != null && posibleVecino.selectable)
			{
				node.posibleNeighbours.Add(posibleVecino);

			}

			if (node.z + 1 < grid.GetHeight())
			{
				if (posibleVecino.isWalkable)
				{
					neighbourList.Add(posibleVecino);
				}
			}

		}
		node.neighbours = neighbourList ;
		return neighbourList;
	}

	bool CheckWall(PathNode originNode, Vector3 direction)
	{
		bool wall = false;
		RaycastHit hiteo;
		Vector3 correccionAltura = new Vector3(0, 0.25f, 0);
		Vector3 center = new Vector3(originNode.worldX + grid.GetCellSize() / 2, 0, originNode.worldZ + grid.GetCellSize() / 2);
		if (Physics.Raycast(center + correccionAltura, direction, out hiteo, grid.GetCellSize(), LayerMask.GetMask("Wall")))
		{
			//Debug.Log("soy " + originNode.ToString() + " y me estoy chocando con " + hiteo.transform.gameObject.name); ;
			//if (hiteo.collider.CompareTag("Muro"))
			//{
				wall = true;
				//CheckRelativeCoverages(hiteo.collider.gameObject);
			//}

		}

		return wall;
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

	public Vector3[] GetSelectableNodesVertsInWorld()
	{
		//List<Vector3> verts = new List<Vector3>();
		//foreach (PathNode pathNode in selectableNodes)
		//{
		//	float offsetY = 0.35f;
		//	Vector3 vert1 = new Vector3(pathNode.worldX, offsetY, pathNode.z);
		//	Vector3 vert2 = new Vector3(pathNode.worldX + grid.GetCellSize(), offsetY, pathNode.worldZ);
		//	Vector3 vert3 = new Vector3(pathNode.worldX , offsetY, pathNode.worldZ + grid.GetCellSize());
		//	Vector3 vert4 = new Vector3(pathNode.worldX + grid.GetCellSize(), offsetY, pathNode.worldZ + grid.GetCellSize());
		//	if (!verts.Contains(vert1))verts.Add(vert1);
		//	if (!verts.Contains(vert2)) verts.Add(vert2);
		//	if (!verts.Contains(vert3)) verts.Add(vert3);
		//	if (!verts.Contains(vert4)) verts.Add(vert4);
		//}
		//return verts.ToArray();

		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < selectableNodes.Count - 1; i++)
		{
			for (int j = i + 1; j < selectableNodes.Count; j++)
			{
				if (AreAdjacent(selectableNodes[i], selectableNodes[j]))
				{
					if (selectableNodes[i].posibleNeighbours.Count <= 3)
					{
						Debug.Log("YIHAAA SOY: " + selectableNodes[i] + " y debo ser una esquina porque mis vecinos son "+selectableNodes[i].posibleNeighbours.Count);
						for (int k = 0; k < selectableNodes[i].posibleNeighbours.Count ; k++)
						{
							Debug.Log("vecino: " + selectableNodes[i].posibleNeighbours[k].ToString());

						}
					}
					float offsetY = 0.35f;
					Vector3 vert1 = new Vector3(selectableNodes[i].worldX, offsetY, selectableNodes[i].worldZ);
					Vector3 vert2 = new Vector3(selectableNodes[i].worldX + grid.GetCellSize(), offsetY, selectableNodes[i].worldZ);
					Vector3 vert3 = new Vector3(selectableNodes[i].worldX, offsetY, selectableNodes[i].worldZ + grid.GetCellSize());
					Vector3 vert4 = new Vector3(selectableNodes[j].worldX + grid.GetCellSize(), offsetY, selectableNodes[j].worldZ + grid.GetCellSize());
					Vector3 vert5 = new Vector3(selectableNodes[j].worldX, offsetY, selectableNodes[j].worldZ + grid.GetCellSize());
					Vector3 vert6 = new Vector3(selectableNodes[j].worldX + grid.GetCellSize(), offsetY, selectableNodes[j].worldZ);

					if (!verts.Contains(vert1)) verts.Add(vert1);
					if (!verts.Contains(vert2)) verts.Add(vert2);
					if (!verts.Contains(vert3)) verts.Add(vert3);
					if (!verts.Contains(vert4)) verts.Add(vert4);
					if (!verts.Contains(vert5)) verts.Add(vert5);
					if (!verts.Contains(vert6)) verts.Add(vert6);
				}
			}
		}
		return verts.ToArray();
	}

	bool AreAdjacent(PathNode node1, PathNode node2)
	{
		int dx = Mathf.Abs(node1.x - node2.x);
		int dz = Mathf.Abs(node1.z - node2.z);
		return (dx == 1 * grid.GetCellSize() && dz == 0) || (dx == 0 && dz == 1 * grid.GetCellSize());
	}

	
	void ClearSelectableNodes()
	{
		if (selectableNodes.Count > 0)
		{
			foreach(PathNode node in selectableNodes)
			{
				node.Reset();
			}
		}
		selectableNodes.Clear();
	}

	public void FindSelectableNodes()
	{
		ClearSelectableNodes();
		//esto a lo mejor solo es para el player principal, ya veremos
		PathNode currentNode = GetCurrentNode();
		//Debug.Log("La casilla actual es: " + currentNode.ToString());
		
		Queue<PathNode> process = new Queue<PathNode>();

		process.Enqueue(currentNode);
		currentNode.visited = true;

		while(process.Count > 0)
		{
			PathNode node = process.Dequeue();
			selectableNodes.Add(node);
			node.selectable = true;

			if (node.distance < CombateManager.instance.GetCurrentUnity().combatData.movementRange)
			{
				foreach(PathNode casilla in GetNeighbourList(node))
				{
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
