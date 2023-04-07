using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathRenderer : MonoBehaviour
{
    public Color lineColor;

    private LineRenderer lineRenderer;

    //private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private GameObject combinedMeshObject;
    public Shader selectableMeshShader;
    private void Start()
    {
        // Agrega un Line Renderer al objeto
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Configura el Line Renderer para usar una textura simple y un material sin textura
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;

        // Establece el color de la línea
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
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

    public void DrawShape(Vector3[] points)
    {
		points = SimplifiedPointsTo3D(SortPointsByAngle(GetSimplifiedPoints(points), new Vector2(0, 0)));

		lineRenderer.positionCount = points.Length;

		for (int i = 0; i < points.Length; i++)
		{
			lineRenderer.SetPosition(i, points[i]);
		}

		lineRenderer.loop = true; // Closes the shape
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

            // Transformamos los vértices del mesh según la transformación del objeto

            // Añadimos el CombineInstance al array
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




        List<Vector3> verticesExteriores = SimplifiedPointsTo3D(SortPoints(GetSimplifiedPoints(GetDistinctPoints(combinedMesh).ToArray()).ToList())).ToList();
        
        //Debug.Log("se supone que tenemos "+ verticesExteriores.Distinct().ToList().Count+" vertices externos");
        //verticesExteriores.Distinct().ToList().ForEach(f => Debug.Log("soy el punto " + f));
        



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
        exteriorNodes.ForEach(f => Debug.Log("soy el nodo exterior " + f));

        Debug.Log("creando el mesh");
		//CombateManager.instance.GetPathfinding().GetSelectableNodesMeshesList();
		List<MeshFilter> meshesList = CombateManager.instance.GetPathfinding().GetSelectableNodesMeshesList();
        if (meshesList != null && meshesList.Count > 0)
		{
            CombineMeshList(meshesList);
        }
    }


    //FUNCIÓN PARA DEBUG
    //void CreateCubes(List<Vector3> positions)
    //{
    //    foreach (Vector3 pos in positions)
    //    {
    //        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //        cube.transform.position = pos;
    //    }
    //}

    List<Vector2> SortPoints(List<Vector2> points)
    {
        List<Vector2> sortedPoints = new List<Vector2>(points);
        sortedPoints.Sort((a, b) =>
        {
            int compareX = a.x.CompareTo(b.x);
            if (compareX != 0)
            {
                return compareX;
            }
            else
            {
                return a.y.CompareTo(b.y);
            }
        });
        return sortedPoints;
    }
    List<Vector3> GetDistinctPoints(Mesh mesh) {
        List<Vector3> allPoints = new List<Vector3>(mesh.vertices);
        List<Vector3> distinctPoints = new List<Vector3>();

        for (int i = 0; i < allPoints.Count; i++) {
            bool alreadyExists = false;
            for (int j = 0; j < i; j++) {
                if (allPoints[i] == allPoints[j]) {
                    alreadyExists = true;
                    break;
                }
            }
            if (!alreadyExists) {
                distinctPoints.Add(allPoints[i]);
            }
        }
        return distinctPoints;
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
