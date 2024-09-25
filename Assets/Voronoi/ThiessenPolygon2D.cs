using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ThiessenPolygon2D<SegmentType, Coord> : PoligonsVoronoi<SegmentVec2<Vector2>, Vector2>
    where SegmentType : Segment<Vector2>, new() where Coord : IEquatable<Vector2>
{
    public ThiessenPolygon2D(Vector2 item, List<Vector2> allIntersections) : base(item, allIntersections)
    {
    }

    public Color colorGizmos;

    public override void AddSegmentsWithLimits(List<SegmentLimit> limits)
    {
        foreach (SegmentLimit limit in limits)
        {
            Vector2 origin = itemSector; // Convert to Vector2
            Vector2 final = limit.GetOpositePosition(origin); // Convert to Vector2

            SegmentVec2<Vector2> segment = new SegmentVec2<Vector2>();
            segment.AddNewSegment(origin, final);
            this.limits.Add(segment);
            segments.Add(segment);
        }
    }

    public override void DrawPoly()
    {
        Vector3[] points = new Vector3[intersections.Count + 1];

        for (int i = 0; i < intersections.Count; i++)
        {
            points[i] = intersections[i];
        }

        points[intersections.Count] = points[0];
        Handles.color = colorGizmos;
        Handles.DrawAAConvexPolygon(points);

        Handles.color = Color.black;
        Handles.DrawPolyLine(points);
    }

    public override bool IsInside(Vector2 point)
    {
        int length = intersections.Count;

        if (length < 3)
        {
            return false;
        }

        Vector2 extreme = new Vector2(100, point.y); // Adjusted for Vector2

        int count = 0;
        for (int i = 0; i < length; i++)
        {
            int next = (i + 1) % length;
            SegmentType intersectionChecker = new SegmentType();
            intersectionChecker.AddNewSegment(Vector2.zero, Vector2.zero);
            Vector2 intersection =
                intersectionChecker.Intersection(intersections[i], intersections[next], point, extreme);
            if (intersection.Equals(Vector2.zero))
                if (IsPointInSegment(intersection, intersections[i], intersections[next]))
                    if (IsPointInSegment(intersection, point, extreme))
                        count++;
        }

        return (count % 2 == 1);
    }

    public override float GetDistance(Vector2 centerCircle, Vector2 segment)
    {
        return Mathf.Sqrt(Mathf.Pow(Mathf.Abs(centerCircle.x - segment.x), 2) +
                          Mathf.Pow(Mathf.Abs(centerCircle.y - segment.y), 2));
    }

    public override bool IsPointInSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        return (point.x <= Mathf.Max(start.x, end.x) &&
                point.x >= Mathf.Min(start.x, end.x) &&
                point.y <= Mathf.Max(start.y, end.y) &&
                point.y >= Mathf.Min(start.y, end.y));
    }
}