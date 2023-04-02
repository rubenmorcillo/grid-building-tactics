using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using Unity.Collections;
using System.Linq;

public class PathRenderer : MonoBehaviour
{
    public Pathfinding pathfinding;
    public Color lineColor;

    private LineRenderer lineRenderer;

    private MeshFilter meshFilter;
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
        lineRenderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            lineRenderer.SetPosition(i, path[i] + new Vector3(0, 0.5f, 0)); // Añade un desplazamiento vertical para que la línea sea visible por encima del terreno
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

	public void CreateSelectableNodesMesh(Vector3[] verts, Material material)
	{

        Debug.Log("tengo " + verts.Length + " puntos");
        ConnectVertices(new List<Vector3>(verts)).ForEach(segment => Debug.Log("SEGMENTO: "+segment));
        //Debug.Log()
        //      int i = 0;
        //      foreach(Vector3 vert in verts)
        //{
        //          i++;
        //          Debug.Log("NORMAL " + i + " ->" + vert);
        //      }
        //verts = SimplifiedPointsTo3D(SortPointsByAngle(GetSimplifiedPoints(verts), new Vector2(CalculateCenter(verts).x, CalculateCenter(verts).z)));
        //i = 0;
        //foreach (Vector3 vert in verts)
        //{
        //    i++;
        //    Debug.Log("ORDENADO "+i+" ->" + vert);

        //}
        List<Vector3> lista = new List<Vector3>(verts);
		CreateMesh(lista.OrderBy(p => p.x).ThenBy(p => p.z).ToList().ToArray());
		List<Vector2> convexHull = GrahamScan.ConvexHull(new List<Vector3>(verts));
        //convexHull.ForEach(c => Debug.Log("CONVEX HULL: "+c));
		//CreateMesh(SimplifiedPointsTo3D(convexHull));

		//Debug.Log("mientras que con el convexHull tendría " + convexHull.Count);

		//i = 0;
		//foreach (Vector2 vert in convexHull)
		//{
		//	i++;
		//	Debug.Log("GRAHAM " + i + " ->" + vert);

		//}
		//      for (int z = 0; z < SimplifiedPointsTo3D(convexHull).Length; z++)
		//      {
		//          Debug.Log("GRAHAM3D " + z + " ->" + SimplifiedPointsTo3D(convexHull)[z]);
		//      }

	}
    public List<(Vector3, Vector3)> ConnectVertices(List<Vector3> vertices)
    {
        List<(Vector3, Vector3)> segments = new List<(Vector3, Vector3)>();

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            Vector3 start = vertices[i];
            Vector3 end = vertices[i + 1];

            // Comprobar si los dos puntos están en la misma fila o columna
            if (start.x == end.x || start.z == end.z)
            {
                // Comprobar si hay obstáculos intermedios
                Vector3 direction = (end - start).normalized;
                float distance = Vector3.Distance(start, end);
                bool hasObstacle = false;

                for (float d = 0f; d < distance; d += 0.1f)
                {
                    Vector3 point = start + direction * d;
                    int x = Mathf.RoundToInt(point.x);
                    int z = Mathf.RoundToInt(point.z);

                    if (CombateManager.instance.GetPathfinding().GetRealNode(x, z) != null)
					{
                        if (!CombateManager.instance.GetPathfinding().GetRealNode(x, z).isWalkable)
                        {
                            hasObstacle = true;
                            break;
                        }
                    }
                   
                }

                if (!hasObstacle)
                {
                    segments.Add((start, end));
                }
            }
        }

        return segments;
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
}
