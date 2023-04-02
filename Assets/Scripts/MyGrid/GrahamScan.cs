using System.Collections.Generic;
using UnityEngine;

public class GrahamScan
{
    public static List<Vector2> ConvexHull(List<Vector3> points)
    {
        // Convertimos las coordenadas XZ a 2D
        List<Vector2> points2D = new List<Vector2>();
        foreach (Vector3 point in points)
        {
            points2D.Add(new Vector2(point.x, point.z));
        }

        // Encontramos el punto más bajo (y el más a la izquierda si hay varios con la misma altura)
        Vector2 lowestPoint = points2D[0];
        foreach (Vector2 point in points2D)
        {
            if (point.y < lowestPoint.y || (point.y == lowestPoint.y && point.x < lowestPoint.x))
            {
                lowestPoint = point;
            }
        }

        // Ordenamos los puntos según el ángulo que forman con el punto más bajo
        points2D.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.y - lowestPoint.y, a.x - lowestPoint.x);
            float angleB = Mathf.Atan2(b.y - lowestPoint.y, b.x - lowestPoint.x);
            if (angleA < angleB)
            {
                return -1;
            }
            else if (angleA > angleB)
            {
                return 1;
            }
            else
            {
                return (a - lowestPoint).magnitude.CompareTo((b - lowestPoint).magnitude);
            }
        });

        // Añadimos el primer punto al stack y recorremos los demás, eliminando los puntos que no forman parte de la envolvente
        Stack<Vector2> hull = new Stack<Vector2>();
        hull.Push(points2D[0]);
        hull.Push(points2D[1]);
        for (int i = 2; i < points2D.Count; i++)
        {
            Vector2 top = hull.Pop();
            while (VectorClockwise(top, hull.Peek(), points2D[i]))
            {
                top = hull.Pop();
            }
            hull.Push(top);
            hull.Push(points2D[i]);
        }

        // Devolvemos la envolvente convexa
        return new List<Vector2>(hull.ToArray());
    }

    // Función auxiliar para determinar si el punto c está a la derecha de la línea ab
    private static bool VectorClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
}