using System;
using System.Collections.Generic;

using System;


[System.Serializable]
public abstract class PoligonsVoronoi<SegmentType, Coord> where Coord : IEquatable<Coord>
    where SegmentType : Segment<Coord>, new()
{
    public bool drawPoli;
    public int weight = 0;
     public Coord itemSector;
     public List<SegmentType> segments = new List<SegmentType>();
     public List<SegmentType> limits = new List<SegmentType>();
     public List<Coord> intersections = new List<Coord>();
     public List<int> indexIntersections = new List<int>();

    protected List<Coord> allIntersections;
   

    public void SortSegment() => segments.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));

    public PoligonsVoronoi(Coord item, List<Coord> allIntersections)
    {
        itemSector = item;
        this.allIntersections = allIntersections;
    }

    public void AddSegment(SegmentType refSegment)
    {
        SegmentType segment = new SegmentType();
        segment.AddNewSegment(refSegment.Origin, refSegment.Final);
        segments.Add(segment);
    }

    public void SetIntersections()
    {
        
        intersections.Clear();
        weight = 0;

        SortSegment();

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < segments.Count; j++)
            {
                if (i == j)
                    continue;
                if (segments[i].id == segments[j].id)
                    continue;

                segments[i].GetTwoPoints(out Coord p1, out Coord p2);
                segments[j].GetTwoPoints(out Coord p3, out Coord p4);

                Coord centerCircle = segments[i].Intersection(p1, p2, p3, p4);

                if (intersections.Contains(centerCircle))
                    continue;

                float maxDistance = GetDistance(centerCircle, segments[i].Origin);

                bool hasOtherPoint = false;
                for (int k = 0; k < segments.Count; k++)
                {
                    if (k == i || k == j)
                        continue;
                    if (HasOtherPointInCircle(centerCircle, segments[k], maxDistance))
                    {
                        hasOtherPoint = true;
                        break;
                    }
                }

                if (!hasOtherPoint)
                {
                    intersections.Add(centerCircle);
                    segments[i].intersection.Add(centerCircle);
                    segments[j].intersection.Add(centerCircle);
                }
            }
        }

        RemoveUnusedSegments();
        SortPointsPolygon();
    }

    public abstract void AddSegmentsWithLimits(List<SegmentLimit> limits);

    private bool HasOtherPointInCircle(Coord centerCircle, SegmentType segment, float maxDistance)
    {
        float distance = GetDistance(centerCircle, segment.Final);
        return distance < maxDistance;
    }

    protected void RemoveUnusedSegments()
    {
        List<SegmentType> segmentsUnused = new List<SegmentType>();
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].intersection.Count != 2)
                segmentsUnused.Add(segments[i]);
        }

        for (int i = 0; i < segmentsUnused.Count; i++)
        {
            segments.Remove(segmentsUnused[i]);
        }
    }

    protected  void SortPointsPolygon()
    {
        intersections.Clear();
        Coord lastIntersection = segments[0].intersection[0];
        intersections.Add(lastIntersection);

        Coord firstIntersection;
        Coord secondIntersection;

        for (int i = 0; i < segments.Count; i++)
        {
            for (int j = 0; j < segments.Count; j++)
            {
                if (i == j)
                    continue;

                firstIntersection = segments[j].intersection[0];
                secondIntersection = segments[j].intersection[1];

                if (!intersections.Contains(secondIntersection))
                    if (firstIntersection.Equals( lastIntersection))
                    {
                        intersections.Add(secondIntersection);
                        lastIntersection = secondIntersection;
                        break;
                    }

                if (!intersections.Contains(firstIntersection))
                    if (secondIntersection.Equals( lastIntersection))
                    {
                        intersections.Add(firstIntersection);
                        lastIntersection = firstIntersection;
                        break;
                    }
            }
        }

        firstIntersection = segments[^1].intersection[0];
        if (!intersections.Contains(firstIntersection))
            intersections.Add(firstIntersection);
        secondIntersection = segments[^1].intersection[1];
        if (!intersections.Contains(secondIntersection))
            intersections.Add(secondIntersection);

        indexIntersections.Clear();
        for (int i = 0; i < intersections.Count; i++)
        {
            Coord intersection = intersections[i];
            if (!allIntersections.Contains(intersection))
            {
                allIntersections.Add(intersection);
                indexIntersections.Add(allIntersections.Count - 1);
            }
            else
            {
                for (int j = 0; j < allIntersections.Count; j++)
                {
                    if (allIntersections[j].Equals(intersection))
                    {
                        indexIntersections.Add(j);
                        break;
                    }
                }
            }
        }

        UpdateIntersectionList();
    }

    protected  void UpdateIntersectionList()
    {
        intersections.Clear();

        for (int i = 0; i < indexIntersections.Count; i++)
        {
            intersections.Add(allIntersections[indexIntersections[i]]);
        }
    }

    public abstract void DrawPoly();



    public abstract bool IsInside(Coord point);

    public abstract float GetDistance(Coord centerCircle, Coord segment);
    
    public abstract bool IsPointInSegment(Coord point, Coord start, Coord end);
}