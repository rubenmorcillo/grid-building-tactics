using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem3D : MonoBehaviour {

    public static GridBuildingSystem3D Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;


    private GridXZ<GridObject> grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;

    [SerializeField]private int gridWidth;
    [SerializeField]private int gridHeight;
    [SerializeField]private float cellSize;

    [SerializeField] private bool showDebug;



    public List<AlreadyPlacedBuild> alreadyPlacedBuilds = new List<AlreadyPlacedBuild>();
    [Serializable]
    public class AlreadyPlacedBuild
    {

        [SerializeField] public PlacedObjectTypeSO tipo;
        [SerializeField] public Vector3 position;
        [SerializeField] public PlacedObjectTypeSO.Dir dir;
        

        public AlreadyPlacedBuild(PlacedObjectTypeSO tipo, Vector3 position, PlacedObjectTypeSO.Dir dir)
        {
            this.tipo = tipo;
            this.position = position;
            this.dir = dir;
        }
    }

    public void ResetManager()
	{
        alreadyPlacedBuilds.Clear();

	}


    private void Awake() {
        Instance = this;
        
        //grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y, showDebug));

        placedObjectTypeSO = null;// placedObjectTypeSOList[0];

        //InitWalls();
        //RecoverPreBuilds();

    }

    public void Init()
	{
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y, showDebug));
        InitWalls();
        RecoverPreBuilds();
    }

    void RecoverPreBuilds()
	{
        List<AlreadyPlacedBuild> alreadyBuilt = alreadyPlacedBuilds;
        //List<Transform> transformBuilds = new List<Transform>();
        foreach (AlreadyPlacedBuild build in alreadyBuilt)
        {

            PlacedObjectTypeSO placedObjectTypeSO = null;
            foreach(PlacedObjectTypeSO placedObjectType in placedObjectTypeSOList)
			{
                if (placedObjectType.nameString.Equals(build.tipo.nameString))
				{
                    placedObjectTypeSO = placedObjectType;
				}
			}

            if (placedObjectTypeSO != null)
			{

                //colocar el edificio que había
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(build.dir);
                //Debug.Log("La rotación es " + rotationOffset);

                Vector3 placedObjectWorldPosition = grid.GetWorldPosition((int)build.position.x, (int)build.position.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                grid.GetXZ(build.position, out int x, out int z);
                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                PlacedObject_Done placedObject = PlacedObject_Done.Create(build.position, placedObjectOrigin, dir, build.tipo);
                //Debug.Log("he llegado hasta aqui con " + build.tipo.nameString);
                //Debug.Log("dice que estaba en " + build.position);
                //Debug.Log("yo lo voy a colocar en la casilla " + placedObjectOrigin);
                //Debug.Log("la posicion en el mundo " + placedObjectWorldPosition);

                //le digo al grid que actualice las posiciones
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }


                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            }
           
        }
    }

    private void InitWalls()
	{
        PlacedObject_Done[] placedObject_Dones = FindObjectsOfType<PlacedObject_Done>();
        //List<Transform> transformsMuros = new List<Transform>();
        foreach (PlacedObject_Done pod in placedObject_Dones)
        {
            Transform t = pod.GetComponent<Transform>();
            grid.GetXZ(t.position, out int x, out int z);
            Vector2Int placedObjectOrigin = new Vector2Int(x, z);

            pod.Setup(placedObjectTypeSOList[3], placedObjectOrigin, dir);

            List<Vector2Int> gridPositionList = pod.GetGridPositionList();

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(pod);
            }

            OnObjectPlaced?.Invoke(this, EventArgs.Empty);
        }
    }

    public class GridObject {

        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;
        bool debugging;

        public GridObject(GridXZ<GridObject> grid, int x, int y, bool debugging) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
            this.debugging = debugging;
        }

        public override string ToString() {
            if (debugging)
			{
                return x + ", " + y + "\n" + placedObject;
			}
			else
			{
                return "";
			}
        }

        public void SetPlacedObject(PlacedObject_Done placedObject) {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject() {
            return placedObject;
        }

        public bool CanBuild() {
            return placedObject == null;
        }

    }

    private void Update() {


        //ESTADO CONSTRUIR
        if (GameManager.instance.gameState == GameManager.GameState.BUILD)
		{
            if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null && !GameManager.instance.uiManager.IsMouseOverUI())
            {
				Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                grid.GetXZ(mousePosition, out int x, out int z);

                Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);

                // Test Can Build
                List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                }

                if (canBuild)
                {
                    Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                        
                    PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                    }
                        
                    //CHAPUZAAA -> RESTAR EL COSTE DE LOS MATERIALES
                    GameManager.instance.resourceManager.cambiarRecursos(new ResourceManager.OnResourceValueChangeArgs( - placedObjectTypeSO.woodCost, "madera"));
                    GameManager.instance.resourceManager.cambiarRecursos(new ResourceManager.OnResourceValueChangeArgs(-placedObjectTypeSO.metalCost, "metal"));
                    //Debug.Log("construyendo " + placedObjectTypeSO.nameString);

                    //CHAPUZAAA -> esto hay que hacerlo bien, no aquí a fuego...pero el tiempo
					if (placedObjectTypeSO.nameString.Equals("Almacen madera"))
					{
                        GameManager.instance.resourceManager.ChangeMaxStorage(new ResourceManager.OnResourceValueChangeArgs (100, "madera"));

                    }else if (placedObjectTypeSO.nameString.Equals("Almacen metal"))
					{
                        GameManager.instance.resourceManager.ChangeMaxStorage(new ResourceManager.OnResourceValueChangeArgs(100, "metal"));
                    }else if (placedObjectTypeSO.nameString.Equals("GeneradorLvl1"))
					{
                        GameManager.instance.gameState = GameManager.GameState.GAMEOVER_WIN;
					}
                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                    alreadyPlacedBuilds.Add(new AlreadyPlacedBuild(placedObjectTypeSO, placedObject.transform.position, dir));
                    DeselectObjectType();

                }
                else
                {
                    // Cannot build here
                    UtilsClass.CreateWorldTextPopup("¡No puedes construir ahí!", mousePosition);
                }
				

			}

            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = PlacedObjectTypeSO.GetNextDir(dir);
            }

            //SelectBuildingToPlace();

        }


        //ESTADO DESTRUIR
        if (GameManager.instance.gameState == GameManager.GameState.DESTROY)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                if (grid.GetGridObject(mousePosition) != null)
                {
                    // Valid Grid Position
                    PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                    if (placedObject != null)
                    {
                        // Demolish
                        Demolish(placedObject);
                    }
                }
            }
        }

		if (GameManager.instance.gameState == GameManager.GameState.BASE)
		{
            DeselectObjectType();
		}

    }

 //   void SelectBuildingToPlace()
	//{
 //       if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
 //       if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
 //       if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
 //       if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
 //       if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
 //       if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }

 //       if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
 //   }
  
    public List<PlacedObjectTypeSO> GetPlacedObjectTypeSOList()
	{
        return placedObjectTypeSOList;
	}
    public void SetPlacedObject(PlacedObjectTypeSO po)
	{
        placedObjectTypeSO = po;
        RefreshSelectedObjectType();
	}

    public void Demolish(PlacedObject_Done placedObject)
	{
        placedObject.DestroySelf();

        List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
        }
    }

    public void DeselectObjectType() {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }

}
