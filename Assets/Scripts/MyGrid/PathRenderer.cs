using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using Unity.Collections;
using System.Linq;
using System;

public class PathRenderer : MonoBehaviour
{
    public Color lineColor;

    private LineRenderer lineRenderer;

    //private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;


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
            // Creamos un CombineInstance y le asignamos el mesh del objeto actual
            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = meshFiltersList[i].mesh;

            // Transformamos los vértices del mesh según la transformación del objeto
            combineInstance.transform = meshFiltersList[i].transform.localToWorldMatrix;

            // Añadimos el CombineInstance al array
            combineInstances[i] = combineInstance;
        }

        // Creamos un nuevo objeto para almacenar el mesh combinado
        GameObject combinedObject = new GameObject("Combined Mesh");

        // Añadimos un componente MeshFilter al objeto
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();

        // Combinamos los meshes en un solo mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true, true);

        // Asignamos el mesh combinado al MeshFilter
        meshFilter.mesh = combinedMesh;

        // Añadimos un componente MeshRenderer al objeto
        meshRenderer = combinedObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

	public void CreateSelectableNodesMesh(Vector3[] verts, Material material)
	{
        //CombateManager.instance.GetPathfinding().GetSelectableNodesMeshesList();
        CombineMeshList(CombateManager.instance.GetPathfinding().GetSelectableNodesMeshesList());

  //      Debug.Log("tengo " + verts.Length + " puntos");
  //      //Debug.Log()
  //      //      int i = 0;
  //      //      foreach(Vector3 vert in verts)
  //      //{
  //      //          i++;
  //      //          Debug.Log("NORMAL " + i + " ->" + vert);
  //      //      }
  //      //verts = SimplifiedPointsTo3D(SortPointsByAngle(GetSimplifiedPoints(verts), new Vector2(CalculateCenter(verts).x, CalculateCenter(verts).z)));
  //      //i = 0;
  //      //foreach (Vector3 vert in verts)
  //      //{
  //      //    i++;
  //      //    Debug.Log("ORDENADO "+i+" ->" + vert);

        //      //}
        //      List<Vector3> lista = new List<Vector3>(verts);
        ////CreateMesh(lista.OrderBy(p => p.x).ThenBy(p => p.z).ToList().ToArray());
        //List<Vector2> convexHull = GrahamScan.ConvexHull(new List<Vector3>(verts));
        ////convexHull.ForEach(c => Debug.Log("CONVEX HULL: "+c));
        //CreateMesh(SimplifiedPointsTo3D(convexHull));

        ////Debug.Log("mientras que con el convexHull tendría " + convexHull.Count);

        ////i = 0;
        ////foreach (Vector2 vert in convexHull)
        ////{
        ////	i++;
        ////	Debug.Log("GRAHAM " + i + " ->" + vert);

        ////}
        ////      for (int z = 0; z < SimplifiedPointsTo3D(convexHull).Length; z++)
        ////      {
        ////          Debug.Log("GRAHAM3D " + z + " ->" + SimplifiedPointsTo3D(convexHull)[z]);
        ////      }

    }

    void CreateMesh(Vector3[] convexHull)
    {
        // Creamos un nuevo Mesh
        Mesh mesh = new Mesh();

        // Añadimos los vertices
        mesh.vertices = convexHull;

        // Definimos los triangulos a partir de los vertices
        int[] triangles = new int[(convexHull.Length - 2) * 3];
        int count = 0;
        for (int i = 2; i < convexHull.Length; i++)
        {
            triangles[count++] = 0;
            triangles[count++] = i - 1;
            triangles[count++] = i;
        }

        // Asignamos los triangulos al Mesh
        mesh.triangles = triangles;

        // Recalculamos las normales y tangentes
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        // Asignamos el Mesh al componente MeshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Configuramos el material del MeshRenderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
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

    public List<Vector3> UnifySegments(List<Vector3> vertices, HashSet<Tuple<Vector3, Vector3>> unitedSegments)
    {
        if (vertices.Count == 1) return vertices;

        var newVertices = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            var v1 = vertices[i];
            for (int j = i + 1; j < vertices.Count; j++)
            {
                var v2 = vertices[j];
                var segment = new Tuple<Vector3, Vector3>(v1, v2);
                if (!unitedSegments.Contains(segment))
                {
                    var canUnify = true;
                    foreach (var otherSegment in unitedSegments)
                    {
                        if (AreSegmentsAdjacent(segment, otherSegment))
                        {
                            var commonVertex = GetCommonVertex(segment, otherSegment);
                            if (commonVertex != null && !vertices.Contains(commonVertex.Value))
                            {
                                canUnify = false;
                                break;
                            }
                        }
                    }

                    if (canUnify)
                    {
                        unitedSegments.Add(segment);
                        newVertices.Add(v1);
                        newVertices.Add(v2);
                    }
                }
            }
        }

        if (newVertices.Count > 0)
        {
            newVertices = UnifySegments(newVertices, unitedSegments);
        }

        return newVertices;
    }

    public bool AreSegmentsAdjacent(Tuple<Vector3, Vector3> segment1, Tuple<Vector3, Vector3> segment2)
    {
        return segment1.Item1 == segment2.Item1 || segment1.Item1 == segment2.Item2 ||
               segment1.Item2 == segment2.Item1 || segment1.Item2 == segment2.Item2;
    }

    public Vector3? GetCommonVertex(Tuple<Vector3, Vector3> segment1, Tuple<Vector3, Vector3> segment2)
    {
        if (segment1.Item1 == segment2.Item1 || segment1.Item1 == segment2.Item2) return segment1.Item1;
        if (segment1.Item2 == segment2.Item1 || segment1.Item2 == segment2.Item2) return segment1.Item2;
        return null;
    }
}
