using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class Voronoid : MonoBehaviour
{
    private float minX, maxX, minY, maxY;
    private List<Cell> cells;
    
    public static List<Cell> GenerateVoronoi(int width, int height, List<Point> points)
    {
        List<Cell> voronoiCells = new List<Cell>();

        foreach (var point in points)
        {
            voronoiCells.Add(new Cell(point));
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point pixel = new Point(x, y);
                Point closestPoint = GetClosestPoint(pixel, points);

                
                var cell = voronoiCells.First(c => c.site.Equals(closestPoint));
                cell.vertices.Add(pixel);
            }
        }

        return voronoiCells;
    }

    private static Point GetClosestPoint(Point pixel, List<Point> points)
    {
        Point closestPoint = points[0];
        double minDistance = Distance(pixel, points[0]);

        foreach (var point in points.Skip(1))
        {
            double dist = Distance(pixel, point);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestPoint = point;
            }
        }

        return closestPoint;
    }
    
    private static double Distance(Point a, Point b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.coord.x - b.coord.x, 2) + Mathf.Pow(a.coord.y - b.coord.y, 2));
    }
}

public class Cell
{
    public Point site;
    public List<Point> vertices;

    public Cell(Point site)
    {
        this.site = site;
    }
}

public class Point
{
    public Vector2 coord;

    public Point(float x, float y)
    {
        coord = new Vector2(x, y);
    }
}