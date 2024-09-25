using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Segment
{
    public static int amountSegments = 0;
    public int id = 0;
    public bool isLimit;
    [SerializeField] private Vector3 origin;
    [SerializeField] private Vector3 final;

    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 mediatrix;
    [SerializeField] private float distance;

    public List<Vector3> intersection = new List<Vector3>();

    public Segment (Vector3 newOrigin, Vector3 newFinal)
    {
        id = amountSegments;
        amountSegments++;
        origin = newOrigin;
        final = newFinal;
        distance = Vector3.Distance(origin, final);

        mediatrix = new Vector3((origin.x + final.x) / 2, (origin.y + final.y) / 2, (origin.z + final.z) / 2);

        direction = new Vector3(final.x - origin.x, final.y - origin.y, final.z - origin.z);
        Vector2 perpendicular = Vector2.Perpendicular(new Vector2(direction.x, direction.z));
        direction.x = perpendicular.x;
        direction.y = 0;
        direction.z = perpendicular.y;
    }

    public Vector3 Direction => direction;
    public Vector3 Mediatrix => mediatrix;
    public Vector3 Origin => origin;
    public Vector3 Final => final;
    public float Distance => distance;

    public void GetTwoPoints (out Vector2 p1, out Vector2 p2)
    {
        p1 = mediatrix;
        p2 = mediatrix + direction * 10;
    }
    
    /// <summary>
    /// Calcula el punto de intersección de 2 rectas.
    /// </summary>
    /// <param name="ap1">Recta 1 Punto 1</param>
    /// <param name="ap2">Recta 1 Punto 2</param>
    /// <param name="bp1">Recta 2 Punto 1</param>
    /// <param name="bp2">Recta 2 Punto 2</param>
    /// <returns></returns>
    public static Vector2 Intersection(Vector2 ap1, Vector2 ap2, Vector2 bp1, Vector2 bp2)
    {
        // https://es.wikipedia.org/wiki/Intersección_de_dos_rectas o https://en.wikipedia.org/wiki/Line–line_intersection

        float denominador = ((ap1.x - ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x - bp2.x));

        Vector2 intersection = Vector2.zero;
        if (denominador == 0)
            return intersection;

        float numeradorX = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.x - bp2.x) - (ap1.x - ap2.x) * (bp1.x * bp2.y - bp1.y * bp2.x));
        float numeradorZ = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x * bp2.y - bp1.y * bp2.x));

        intersection.x = numeradorX / denominador;
        intersection.y = numeradorZ / denominador;

        return intersection;
    }

    public void DrawSegment (float distanceSegment)
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(mediatrix, direction * distanceSegment);
        Gizmos.DrawRay(mediatrix, -direction * distanceSegment);
    }
}