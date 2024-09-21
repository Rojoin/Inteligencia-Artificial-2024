using System;
using System.Collections.Generic;
using UnityEngine;

public class Voronoid<NodeType, Coordinate> where NodeType : INode<Coordinate>
    where Coordinate : IEquatable<Coordinate>
{
    private List<NodeType> voronoiCenters;
    private Dictionary<NodeType, List<Vector2>> voronoiPolygons;
    private int gridWidth, gridHeight;
    private IGraph<NodeType,Coordinate> graph;

    public Voronoid(List<NodeType> centers, int width, int height,IGraph<NodeType,Coordinate> graph)
    {
        voronoiCenters = centers;
        voronoiPolygons = new Dictionary<NodeType, List<Vector2>>();
        gridWidth = width;
        gridHeight = height;
        this.graph = graph;
    }
    
    //Node scale * quatity + (sepation* cuantity -1)
    public List<Cell<NodeType,Coordinate>> GenerateVoronoi(int width, int height, List<NodeType> points)
    {
        List<Cell<NodeType,Coordinate>> voronoiCells = new List<Cell<NodeType,Coordinate>>();

        foreach (var point in points)
        {
            voronoiCells.Add(new Cell<NodeType,Coordinate>(point));
        }

        foreach (Cell<NodeType,Coordinate> voronoiCell in voronoiCells)
        {
            foreach (Cell<NodeType,Coordinate> cell in voronoiCells)
            {
                if (cell == voronoiCell)
                {
                    continue;
                }
                                 
                voronoiCell.vertices.Add(new Point<Coordinate>(graph.GetMediatrix(cell.site,voronoiCell.site)));
            }
        }
        return voronoiCells;
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

public class Point<Coordinate>
{
    public Coordinate coord;

    public Point(Coordinate coord)
    {
        this.coord = coord;
    }
}

