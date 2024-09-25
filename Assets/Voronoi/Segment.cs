using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    public static int amountSegments = 0;
    public int id = 0;
    public bool isLimit;

    [SerializeField] private Vector2 origin;
    [SerializeField] private Vector2 final;

    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 mediatrix;
    [SerializeField] private float distance;

    public List<Vector2> intersection = new List<Vector2>();

    public Segment(Vector2 newOrigin, Vector2 newFinal)
    {
        id = amountSegments;
        amountSegments++;
        origin = newOrigin;
        final = newFinal;
        distance = Vector2.Distance(origin, final);

        mediatrix = (origin + final) / 2;

        direction = (final - origin).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x); // Perpendicular in 2D
        direction = perpendicular; // Set direction to be the perpendicular
    }

    public Vector2 Direction => direction;
    public Vector2 Mediatrix => mediatrix;
    public Vector2 Origin => origin;
    public Vector2 Final => final;
    public float Distance => distance;

    public void GetTwoPoints(out Vector2 p1, out Vector2 p2)
    {
        p1 = mediatrix;
        p2 = mediatrix + direction * 10;
    }

    /// <summary>
    /// Calculates the intersection point of two lines in 2D.
    /// </summary>
    /// <param name="ap1">Line 1 Point 1</param>
    /// <param name="ap2">Line 1 Point 2</param>
    /// <param name="bp1">Line 2 Point 1</param>
    /// <param name="bp2">Line 2 Point 2</param>
    /// <returns>The intersection point, or Vector2.zero if lines are parallel.</returns>
    public static Vector2 Intersection(Vector2 ap1, Vector2 ap2, Vector2 bp1, Vector2 bp2)
    {
        float denominator = ((ap1.x - ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x - bp2.x));

        if (denominator == 0)
            return Vector2.zero; // Lines are parallel

        float numeradorX = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.x - bp2.x) - (ap1.x - ap2.x) * (bp1.x * bp2.y - bp1.y * bp2.x));
        float numeradorY = ((ap1.x * ap2.y - ap1.y * ap2.x) * (bp1.y - bp2.y) - (ap1.y - ap2.y) * (bp1.x * bp2.y - bp1.y * bp2.x));

        Vector2 intersection = new Vector2(numeradorX / denominator, numeradorY / denominator);
        return intersection;
    }

    public void DrawSegment(float distanceSegment)
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(mediatrix, mediatrix + direction * distanceSegment);
        Gizmos.DrawLine(mediatrix, mediatrix - direction * distanceSegment);
    }
}
