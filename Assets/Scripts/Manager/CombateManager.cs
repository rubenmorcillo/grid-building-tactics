using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombateManager : MonoBehaviour
{
    private static CombateManager _instance;

    public static CombateManager instance
    {
        get
        {
            return _instance;
        }
    }



    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;

    [SerializeField] private GameObject tileModel;
    [SerializeField] private MovingUnitTest movingUnitTest;
    [SerializeField] private bool showDebug;
    [SerializeField] private bool crearTilesMapa;
    private GridXZ<GridBuildingSystem3D.GridObject> grid;

    private Pathfinding pathfinding;

    [SerializeField] TacticUnity currentUnity;
    [SerializeField] TurnManager turnManager;
    [SerializeField] PathRenderer pathRenderer;


    void Start()
    {
        _instance = this;
        if (crearTilesMapa)
        {
            grid = new GridXZ<GridBuildingSystem3D.GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (GridXZ<GridBuildingSystem3D.GridObject> g, int x, int y) => new GridBuildingSystem3D.GridObject(g, x, y, showDebug));
            CrearTilesMapa();
        }
        pathfinding = new Pathfinding(gridWidth, gridHeight, cellSize);
        if (turnManager == null)
        {
            turnManager = new TurnManager();
        }
        if (pathRenderer == null)
		{
            pathRenderer = gameObject.AddComponent<PathRenderer>();
		}

        //CHAPUZAAA temporal: TODO -> se deben recuperar todas las unidades al inicio del combate
        turnManager.AddUnit(currentUnity);
    }


    void FixedUpdate()
    {
      
        pathfinding.FindSelectableNodes();
        pathfinding.DebugDrawSelectables();


        turnManager?.Update();

        ControlarClick();




    }

    //CHAPUZAAA temporal: TODO -> un manager de input(?)
    void ControlarClick()
    {
        pathfinding.GetGrid().GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);
        if (pathfinding.GetGrid().GetGridObject(x, z).selectable)
        {
            List<PathNode> path = pathfinding.FindPath(pathfinding.GetCurrentNode().x, pathfinding.GetCurrentNode().z, x, z);


            //DEBUG DRAWLINE
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector3 offset = new Vector3(pathfinding.GetGrid().GetCellSize() / 2, 0, pathfinding.GetGrid().GetCellSize() / 2);
                    pathRenderer.DrawPath(pathfinding.pathToVectorArray(path));
                    //Debug.DrawLine(new Vector3(path[i].x, 0.5f, path[i].z) * pathfinding.GetGrid().GetCellSize() + offset, new Vector3(path[i + 1].x, 0.5f, path[i + 1].z) * pathfinding.GetGrid().GetCellSize() + offset, Color.green);
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            //Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPositionWithZ();
         
            movingUnitTest.SetTargetPosition(Mouse3D.GetMouseWorldPosition());
        }

    }
    public Pathfinding GetPathfinding()
	{
        return pathfinding;
	}
    public void SetCurrentUnity(TacticUnity currentUnity)
	{
        this.currentUnity = currentUnity;
        pathfinding.FindSelectableNodes();
	}
    public TacticUnity GetCurrentUnity()
	{
        return currentUnity;
	}
    
    void CrearTilesMapa()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject tile = Instantiate(tileModel, grid.GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f, Quaternion.identity);
                tile.transform.localScale *= cellSize;
                //tile.AddComponent<Tile>
                //Debug.DrawLine(grid.GetWorldPosition(x, z), grid.GetWorldPosition(x, z + 1), Color.white, 100f);
                //Debug.DrawLine(grid.GetWorldPosition(x, z), grid.GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
    }
    public void EndTurn()
	{
        TurnManager.EndTurn();
	}

    public void Attack()
	{
        currentUnity.combatData.actionDone = true;
	}
}
