using System.Collections.Generic;
using UnityEngine;

public class PolygonGenerator : MonoBehaviour
{
    //public GameObject tilePrefab;
    //public int numRows = 5;
    //public int numColumns = 5;
    //public float tileSpacing = 1f;

    //private List<Vector3> vertices = new List<Vector3>();

    //private void Start()
    //{
    //    // Generate the grid of tiles
    //    for (int row = 0; row < numRows; row++)
    //    {
    //        for (int column = 0; column < numColumns; column++)
    //        {
    //            float x = column * tileSpacing;
    //            float z = row * tileSpacing;

    //            GameObject tile = Instantiate(tilePrefab, new Vector3(x, 0f, z), Quaternion.identity);
    //            tile.transform.SetParent(transform);
    //        }
    //    }

    //    // Generate a list of vertices from the tile positions
    //    foreach (Transform child in transform)
    //    {
    //        vertices.Add(child.position);
    //    }

    //    // Sort the vertices using the Convex Hull algorithm
    //    List<Vector3> sortedVertices = ConvexHull.GetConvexHull(vertices);

    //    // Create a mesh from the sorted vertices
    //    Mesh mesh = new Mesh();
    //    mesh.vertices = sortedVertices.ToArray();

    //    // Generate the triangles for the mesh
    //    List<int> triangles = new List<int>();

    //    for (int i = 0; i < sortedVertices.Count - 2; i++)
    //    {
    //        triangles.Add(0);
    //        triangles.Add(i + 1);
    //        triangles.Add(i + 2);
    //    }

    //    mesh.triangles = triangles.ToArray();

    //    // Create a game object to display the mesh
    //    GameObject polygon = new GameObject("Polygon");
    //    polygon.AddComponent<MeshRenderer>();
    //    polygon.AddComponent<MeshFilter>();
    //    polygon.GetComponent<MeshFilter>().mesh = mesh;
    //    polygon.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
    //}
}