using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    public Pathfinding pathfinding;
    public Color lineColor;

    private LineRenderer lineRenderer;

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
}
