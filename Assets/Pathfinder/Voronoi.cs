using System;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi<NodeType, Coordinate> where NodeType : INode<Coordinate>
    where Coordinate : IEquatable<Coordinate>
{
    private List<Node<Vector2>> voronoiCenters;
    public Dictionary<Node<Vector2>, List<Segment<Vector2>>> voronoiPolygons;
    private int gridWidth, gridHeight;
    private IGraph<NodeType, Coordinate> graph;

    private Coordinate min;
    private Coordinate max;
    private float padding = 0.5f;
    public static readonly Vector2 INVALID_VALUE = new(Single.MaxValue, Single.MinValue);

    public Voronoi(List<Node<Vector2>> centers, int width, int height, IGraph<NodeType, Coordinate> graph)
    {
        voronoiCenters = centers;
        voronoiPolygons = new Dictionary<Node<Vector2>, List<Segment<Vector2>>>();
        gridWidth = width;
        gridHeight = height;
       // this.graph = graph;
    }

    //Node scale * quatity + (sepation* cuantity -1)
    public void GenerateVoronoi()
    {
        foreach (Node<Vector2> center in voronoiCenters)
        {
            List<Segment<Vector2>> voronoiPoints = new List<Segment<Vector2>>();
            
            Point<Vector2> upLeft = new(new Vector2(-padding*2, gridHeight + padding));
            Point<Vector2> upRight = new(new Vector2(gridWidth + padding, gridHeight + padding));
            Point<Vector2> downRight = new(new Vector2(gridWidth + padding, -padding*2));
            Point<Vector2> downLeft = new(new Vector2(-padding *2, -padding*2));

            Segment<Vector2> upSegment = new(upLeft, upRight);
            Segment<Vector2> rightSegment = new(upRight, downRight);
            Segment<Vector2> downSegment = new(downRight, downLeft);
            Segment<Vector2> leftSegment = new(downLeft, upLeft);

            upLeft.AddSegments(upSegment, leftSegment);
            upRight.AddSegments(rightSegment,upSegment);
            downRight.AddSegments(downSegment, rightSegment);
            downLeft.AddSegments(leftSegment,downSegment);

            voronoiPoints.Add(upSegment);
            voronoiPoints.Add(rightSegment);
            voronoiPoints.Add(downSegment);
            voronoiPoints.Add(leftSegment);

            voronoiPolygons.Add(center,voronoiPoints);
            // voronoiPolygons[center] = voronoiPoints;
        }

        for (int index = 0; index < voronoiCenters.Count; index++)
        {
            for (int j = 0; j < voronoiCenters.Count; j++)
            {
                if (index == j)
                {
                    continue;
                }

                //For the weighted i need to move the point according to the wight of the polygons 
                Vector2 director = GetDirectorVector(index, j);

                CalculatePolygon(voronoiCenters[index], director);

            }
        }
        
    }

    private Vector2 GetDirectorVector(int index, int j)
    {
        Vector2 A = voronoiCenters[index].GetCoordinate();
        Vector2 B = voronoiCenters[j].GetCoordinate();

        Vector2 direction = B - A;
        Vector2 director = new Vector2(-direction.y, direction.x).normalized;

        float distance = Vector2.Distance(A, B);
        //float distance = Mathf.Sqrt(Mathf.Pow(A.x - B.x, 2) +
        //                            Mathf.Pow(Mathf.Abs(A.y - B.y), 2));


        director *= (distance * 0.5f);
        return director;
    }

    public bool LineVectorCollision(Segment<Vector2> line, Vector2 dir, out Vector2 intersection)
    {
        Vector2 startPoint = new Vector2(-1, -1);
        Vector2 lineDir = line.end.coord - line.init.coord;
    
        // Calculate the denominator of the intersection formula
        float denom =   dir.y * lineDir.x-dir.x * lineDir.y;

        // Check if the lines are parallel (denom close to 0)
        if (Mathf.Abs(denom) < Mathf.Epsilon)
        {
            intersection = INVALID_VALUE;
            return false;
        }

        // Calculate the uA and uB values
        float uA = ((dir.x * (line.init.coord.y - startPoint.y)) - (dir.y * (line.init.coord.x - startPoint.x))) / denom;
        float uB = ((lineDir.x * (line.init.coord.y - startPoint.y)) - (lineDir.y * (line.init.coord.x - startPoint.x))) / denom;

        // Check if the intersection is within the bounds of the segment
        if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
        {
            // Calculate the intersection point using uA
             intersection = new Vector2(
                 line.init.coord.x + uA * lineDir.x,
                 line.init.coord.y + uA * lineDir.y);
            //intersection = startPoint + dir.normalized * uA;
            return true;
        }

        intersection = INVALID_VALUE;
        return false;
    }

    void CalculatePolygon(Node<Vector2> center, Vector2 dir)
    {
        List<Vector2>  interceptionPoints = new List<Vector2>();
        List<Point<Vector2>> pointsA = new List<Point<Vector2>>();
        List<Point<Vector2>> pointsB = new List<Point<Vector2>>();
        List<Segment<Vector2>> polygonA = new(voronoiPolygons[center]);
        List<Segment<Vector2>> polygonB = new(voronoiPolygons[center]);
        for (int i = 0; i < voronoiPolygons[center].Count; i++)
        {
            Segment<Vector2> segment = voronoiPolygons[center][i];
            //TOdo:Change so the Polygons are created correctly
            if (LineVectorCollision(segment, dir, out Vector2 inter) && inter != INVALID_VALUE)
            {
                Segment<Vector2> aInter = new Segment<Vector2>(segment.init, new Point<Vector2>(inter));
                segment.init.Segments[0] = aInter;
                aInter.end.Segments.Add(aInter);
                Segment<Vector2> bInter = new Segment<Vector2>(new Point<Vector2>(inter), segment.end);
                segment.end.Segments[0] = bInter;
                bInter.init.Segments.Add(bInter);
                polygonA[i] = aInter;
                polygonB[i] = bInter;
                interceptionPoints.Add(inter);
                pointsA.Add(aInter.end);
                pointsB.Add(bInter.init);
                
            }
        }

        if (interceptionPoints.Count == 2)
        {
            Segment<Vector2> aInter = new Segment<Vector2>(pointsA[0], pointsA[1]);
            pointsA[0].Segments.Add(aInter);
            pointsA[1].Segments.Add(aInter);
            Segment<Vector2> bInter = new Segment<Vector2>(pointsB[0], pointsB[1]);
            pointsB[0].Segments.Add(aInter);
            pointsB[1].Segments.Add(aInter);
            polygonA.Add(aInter);
            polygonB.Add(bInter);
            if (IsPointInPolygon(center.GetCoordinate(),polygonA))
            {
                voronoiPolygons[center] = polygonA;
            }
            else
            {
                voronoiPolygons[center] = polygonB;
            }
        }
        
    }


    public bool IsPointInPolygon(Vector2 point, List<Segment<Vector2>> polygonSegments)
    {
        int intersectionCount = 0;
        Vector2 dir = new Vector2(1, 0);

        foreach (var segment in polygonSegments)
        {
            Vector2 intersection;
            if (LineVectorCollision(segment, dir, out intersection) && intersection != INVALID_VALUE)
            {
                if (IsPointOnSegment(segment, intersection) && intersection.x > point.x)
                {
                    intersectionCount++;
                }
            }
        }

        return (intersectionCount % 2 == 1);
    }

    private bool IsPointOnSegment(Segment<Vector2> segment, Vector2 point)
    {
        return Mathf.Min(segment.init.coord.x, segment.end.coord.x) <= point.x
               && point.x <= Mathf.Max(segment.init.coord.x, segment.end.coord.x)
               && Mathf.Min(segment.init.coord.y, segment.end.coord.y) <= point.y
               && point.y <= Mathf.Max(segment.init.coord.y, segment.end.coord.y);
    }
}

public class Cell<NodeType, Coordinate> where NodeType : INode<Coordinate> where Coordinate : IEquatable<Coordinate>
{
    public NodeType site;
    public List<Point<Coordinate>> vertices;

    public Cell(NodeType site)
    {
        this.site = site;
    }
}

public class Segment<Coordinate>
{
    public Point<Coordinate> init;
    public Point<Coordinate> end;

    public Segment(Coordinate init, Coordinate end)
    {
        this.init = new Point<Coordinate>(init);
        this.end = new Point<Coordinate>(end);
    }

    public Segment(Point<Coordinate> init, Point<Coordinate> end)
    {
        this.init = init;
        this.end = end;
    }
}

public class Point<Coordinate>
{
    public Coordinate coord;
    public List<Segment<Coordinate>> Segments = new List<Segment<Coordinate>>();

    public Point(Coordinate coord)
    {
        this.coord = coord;
    }

    public void AddSegments(Segment<Coordinate> segment1, Segment<Coordinate> segment2)
    {
        Segments.Add(segment1);
        Segments.Add(segment2);
    }
}