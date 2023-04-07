using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    

    //private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private GameObject combinedMeshObject;
    public Shader selectableMeshShader;
	private void Awake()
	{
        lineRenderer = gameObject.AddComponent<LineRenderer>();

    }
    private void Start()
    {
        // Agrega un Line Renderer al objeto

        // Configura el Line Renderer para usar una textura simple y un material sin textura
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;

        // Establece el color de la línea
        lineRenderer.startColor = CombateManager.instance.lineRendererColor;
        lineRenderer.endColor = CombateManager.instance.lineRendererColor;
    }


    public void DrawPath(Vector3[] path)
    {
        // Configura el Line Renderer para usar los puntos de la línea que se están dibujando en el script
        if (lineRenderer != null)
		{
            lineRenderer.positionCount = path.Length;
            for (int i = 0; i < path.Length; i++)
            {
                lineRenderer.SetPosition(i, path[i] + new Vector3(0, 0.5f, 0)); // Añade un desplazamiento vertical para que la línea sea visible por encima del terreno
            }
        }
        
    }

    public void CombineMeshList(List<MeshFilter> meshFiltersList)
	{
        CombineInstance[] combineInstances = new CombineInstance[meshFiltersList.Count];

        // Recorremos la lista de objetos
        for (int i = 0; i < meshFiltersList.Count; i++)
        {
            //Debug.Log("Soy el mesh de " + meshFiltersList[i].gameObject);
            // Creamos un CombineInstance y le asignamos el mesh del objeto actual
            CombineInstance combineInstance = new CombineInstance();
            if (meshFiltersList[i] != null)
			{
                combineInstance.mesh = meshFiltersList[i].mesh;
                combineInstance.transform = meshFiltersList[i].transform.localToWorldMatrix;

            }
            combineInstances[i] = combineInstance;
        }

        //si ya existe lo destruímos
        if (combinedMeshObject != null)
		{
            Destroy(combinedMeshObject);
		}
        // Creamos un nuevo objeto para almacenar el mesh combinado
        combinedMeshObject = new GameObject("Combined Mesh");

        // Añadimos un componente MeshFilter al objeto
        MeshFilter meshFilter = combinedMeshObject.AddComponent<MeshFilter>();

        // Combinamos los meshes en un solo mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);


		// Asignamos el mesh combinado al MeshFilter
		meshFilter.mesh = combinedMesh;

        // Añadimos un componente MeshRenderer al objeto
        meshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();

        //Load a Texture (Assets/Resources/Textures/texture01.png)
        Material material  = Resources.Load<Material>("Materials/OutlineMaterial");
        meshRenderer.material = material;

    }


    public void CreateSelectableNodesMesh()
	{
		List<PathNode> exteriorNodes = CombateManager.instance.GetPathfinding().GetExteriorSelectableNodes();

        //CreateCubes(GetExteriorPoints(exteriorNodes).Distinct().ToList());
        CreateCubes(Ordenar(GetExteriorPoints(exteriorNodes).Distinct().ToList()));


		DrawPath(Ordenar(GetExteriorPoints(exteriorNodes).Distinct().ToList()).ToArray());
		//exteriorNodes.ForEach(f => Debug.Log("soy el nodo exterior " + f));


		Debug.Log("creando el mesh");
		List<MeshFilter> meshesList = CombateManager.instance.GetPathfinding().GetSelectableNodesMeshesList();
		if (meshesList != null && meshesList.Count > 0)
		{
			CombineMeshList(meshesList);
		}
	}

   
    List<Vector3> GetExteriorPoints(List<PathNode> pathNodeList)
	{
        float cellSize = CombateManager.instance.GetPathfinding().GetGrid().GetCellSize();
        List<Vector3> exteriorPoints = new List<Vector3>();
        foreach(PathNode pathNode in pathNodeList)
		{
            foreach (Directions.Cardinal ladoExterno in pathNode.exterior)
            {
                Vector3 pointA; 
                Vector3 pointB;
                switch (ladoExterno)
                {
                    case Directions.Cardinal.ESTE:
                        pointA = new Vector3(pathNode.worldX + 1 * cellSize, 0, pathNode.worldZ + 1 * cellSize);
                        pointB = new Vector3(pathNode.worldX + 1 * cellSize, 0, pathNode.worldZ + 1 * cellSize);
                        exteriorPoints.Add(pointA);
                        exteriorPoints.Add(pointB);
                        break;
                    case Directions.Cardinal.OESTE:
                        pointA = new Vector3(pathNode.worldX, 0, pathNode.worldZ);
                        pointB = new Vector3(pathNode.worldX, 0, pathNode.worldZ + 1 * cellSize);
                        exteriorPoints.Add(pointA);
                        exteriorPoints.Add(pointB);
                        break;
                    case Directions.Cardinal.NORTE:
                        pointA = new Vector3(pathNode.worldX, 0, pathNode.worldZ + 1 * cellSize);
                        pointB = new Vector3(pathNode.worldX + 1 * cellSize, 0, pathNode.worldZ + 1 * cellSize);
                        exteriorPoints.Add(pointA);
                        exteriorPoints.Add(pointB);
                        break;
                    case Directions.Cardinal.SUR:
                        pointA = new Vector3(pathNode.worldX, 0, pathNode.worldZ);
                        pointB = new Vector3(pathNode.worldX + 1 * cellSize, 0, pathNode.worldZ);
                        exteriorPoints.Add(pointA);
                        exteriorPoints.Add(pointB);
                        break;
                }
            }
            
        }
        return exteriorPoints;
    }

	//FUNCIÓN PARA DEBUG
	void CreateCubes(List<Vector3> positions)
	{
		foreach (Vector3 pos in positions)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = pos;
		}
	}
    List<Vector3> Ordenar(List<Vector3> originalPoints)
	{
        float cellSize = CombateManager.instance.GetPathfinding().GetGrid().GetCellSize();
        List<Vector3> orderedPoints = new List<Vector3>();
        orderedPoints.Add(originalPoints.First());
        for(int i = 0; i < originalPoints.Count; i++)
		{
            for (int j = i + 1; j < originalPoints.Count; j++)
			{
                Vector3 puntoA = originalPoints[i];
                Vector3 puntoB = originalPoints[j];
                Debug.Log(" vamos a comparar el punto "+ puntoA+ " y el punto " + puntoB);
                float diferenciaX = Math.Abs(puntoA.x - puntoB.x);
                float diferenciaZ = Math.Abs(puntoA.z - puntoB.z);
                float sumaDiferencia = diferenciaX + diferenciaZ;
                Debug.Log("la diferncia total es de " + sumaDiferencia);
                if (sumaDiferencia == cellSize)
				{
                    Debug.Log("Este me cuadra");
                    if (!orderedPoints.Contains(puntoB))
					{
                        Debug.Log("añadiendo a la lista ordenada " + puntoB);

                        orderedPoints.Add(puntoB);
                        break;
                    }
                }
            }
		}

        //orderedPoints.ForEach(o => Debug.Log("Soy el punto ordenado " + o));
        return orderedPoints;

    }
	
    
    // Ordena los puntos por ángulo respecto al punto pivot
    private static List<Vector2> SortPointsByAngle(List<Vector2> points, Vector2 pivot)
    {
        List<Vector2> sortedPoints = new List<Vector2>(points);

        // Calcula el ángulo de cada punto respecto al punto pivot
        sortedPoints.Sort((p1, p2) => {
            float angle1 = GetAngleBetweenPoints(pivot, p1);
            float angle2 = GetAngleBetweenPoints(pivot, p2);

            return angle1.CompareTo(angle2);
        });

        return sortedPoints;
    }

    // Devuelve el ángulo en grados entre dos puntos respecto a la horizontal
    private static float GetAngleBetweenPoints(Vector2 point1, Vector2 point2)
    {
        float angle = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    List<Vector2> GetSimplifiedPoints(Vector3[] points)
    {
        List<Vector2> simplifiedPoints = new List<Vector2>();
        foreach (Vector3 point in points)
        {
            simplifiedPoints.Add(new Vector2(point.x, point.z));
        }
        return simplifiedPoints;
    }

    Vector3[] SimplifiedPointsTo3D(List<Vector2> simplifiedPoints)
	{
        List<Vector3> points3D = new List<Vector3>();
        foreach(Vector2 simplifiedPoint in simplifiedPoints)
		{
            float offsetY = 0.35f;
            points3D.Add(new Vector3(simplifiedPoint.x, offsetY, simplifiedPoint.y));
		}
        
        return points3D.ToArray();
	}

    

}
