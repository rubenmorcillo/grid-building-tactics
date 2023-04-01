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

        //CHAPUZAAA temporal: TODO -> se deben recuperar todas las unidades al inicio del combate
        turnManager.AddUnit(currentUnity);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pathfinding.FindSelectableNodes();
        }
        pathfinding.DebugDrawSelectables();


        turnManager?.Update();

		

        if (Input.GetMouseButton(0))
        {
            //Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPositionWithZ();
            pathfinding.GetGrid().GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, z);
            //DEBUG DRAWLINE
            //         if ( path != null)
            //{
            //             for (int i=0; i<path.Count - 1; i++) {
            //                 Debug.DrawLine(new Vector3(path[i].x + 0.5f, 0.5f, path[i].z +0.5f) * grid.GetCellSize() , new Vector3(path[i + 1].x +0.5f, 0.5f, path[i + 1].z + 0.5f) * grid.GetCellSize(), Color.green); 
            //	}
            //}
            movingUnitTest.SetTargetPosition(Mouse3D.GetMouseWorldPosition());
        }

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
   
}
