using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
	[SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
	private PlacedObjectTypeSO placedObjectTypeSO;
	private GridXZ<GridObject> grid;
	private PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

	private void Awake()
	{
		int gridWith = 10;
		int gridHeight = 10;
		int cellSize = 10;
		grid = new GridXZ<GridObject>(gridWith, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

		placedObjectTypeSO = placedObjectTypeSOList[0];
	}


	public class GridObject
	{
		private GridXZ<GridObject> grid;
		public int x;
		public int z;
		private PlacedObject_Done placedObject;

		public GridObject(GridXZ<GridObject> grid, int x, int z)
		{
			this.grid = grid;
			this.x = x;
			this.z = z;
		}

		public void SetPlacedObject(PlacedObject_Done placedObject)
		{
			this.placedObject = placedObject;
			grid.TriggerGridObjectChanged(x, z);
		}

		public void ClearPlacedObject()
		{
			placedObject = null;
			grid.TriggerGridObjectChanged(x, z);

		}

		public PlacedObject_Done GetPlacedObject()
		{
			return placedObject;
		}

		public bool CanBuild()
		{
			return placedObject == null;
		}

		public override string ToString()
		{
			return x + ", " + z + "\n" + placedObject;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

			List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

			bool canBuild = true;
			foreach (Vector2Int gridPosition in gridPositionList)
			{
				if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
				{
					canBuild = false;
					break;
				}
				
			}
			GridObject gridObject = grid.GetGridObject(x, z);
			if (canBuild)
			{
				Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
				Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();


				PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);
				
				
				foreach(Vector2Int gridPosition in gridPositionList)
				{
					grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
				}
			}
			else
			{
				UtilsClass.CreateWorldTextPopup("Cannot build here!", Mouse3D.GetMouseWorldPosition());
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			GridObject gridObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
			PlacedObject_Done placedObject = gridObject.GetPlacedObject();
			if (placedObject != null)
			{
				placedObject.DestroySelf();

				List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
				foreach (Vector2Int gridPosition in gridPositionList)
				{
					grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
				}

			}
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			dir = PlacedObjectTypeSO.GetNextDir(dir);
			UtilsClass.CreateWorldTextPopup(""+ dir, Mouse3D.GetMouseWorldPosition());
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; };
		if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; };
		if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; };
		
		
	}
}
