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
    [SerializeField]  public float persentageOfDistance;

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
    
    public abstract Coord Intersection(Coord ap1, Coord ap2, Coord bp1, Coord bp2);

    public abstract void AddNewSegment(Coord newOrigin, Coord newFinal, float persentageOfDistance);

}