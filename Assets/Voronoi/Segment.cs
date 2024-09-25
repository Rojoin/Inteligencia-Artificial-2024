using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Segment<Coord> 
{
    public static int amountSegments = 0;
    public int id = 0;
    public bool isLimit;

    [SerializeField] protected Coord origin;
    [SerializeField] protected Coord final;

    [SerializeField] protected Coord direction;
    [SerializeField] protected Coord mediatrix;
    [SerializeField]  protected float distance;

    public List<Coord> intersection = new List<Coord>();

    public Segment(Coord newOrigin, Coord newFinal)
    {
        
    }public Segment()
    {
        
    }
    public Coord Direction => direction;
    public Coord Mediatrix => mediatrix;
    public Coord Origin => origin;
    public Coord Final => final;
    public float Distance => distance;

    public abstract void GetTwoPoints(out Coord p1, out Coord p2);

    /// <summary>
    /// Calculates the intersection point of two lines in 2D.
    /// </summary>
    /// <param name="ap1">Line 1 Point 1</param>
    /// <param name="ap2">Line 1 Point 2</param>
    /// <param name="bp1">Line 2 Point 1</param>
    /// <param name="bp2">Line 2 Point 2</param>
    /// <returns>The intersection point, or Vector2.zero if lines are parallel.</returns>
    public abstract Coord Intersection(Coord ap1, Coord ap2, Coord bp1, Coord bp2);

    public abstract void AddNewSegment(Coord newOrigin, Coord newFinal);
    // public void DrawSegment(float distanceSegment)
    // {
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(mediatrix, mediatrix + direction * distanceSegment);
    //     Gizmos.DrawLine(mediatrix, mediatrix - direction * distanceSegment);
    // }
}