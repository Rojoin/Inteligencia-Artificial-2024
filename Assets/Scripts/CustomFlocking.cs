﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomFlocking : MonoBehaviour
{
    public Transform target;
    // public int boidCount = 50;
    public Boid boidPrefab;
    public List<Agent> agents;
    private List<BoidAgent> boids = new List<BoidAgent>();
    public float detectionRadious = 3.0f;
    public float aligmentWeight = 1;
    public float cohesionWeight = 1.5f;
    public float separationWeight = 2;
    public float speed = 2;
    

    [ContextMenu("RaiseAlarm")]
    public void RaiseAlarm()
    {
         foreach (Agent agent in agents)
         {
             agent.onAlarmRaised?.Invoke();
         }
    } 
    [ContextMenu("StopAlarm")]
    public void StopAlarm()
    {
        foreach (Agent agent in agents)
        {
            agent.onAlarmStop.Invoke();
        }
    }
    private void Start()
    {
        foreach (var VARIABLE in agents)
        {
            BoidAgent boid = VARIABLE.boid;
            boid.Init(Alignment, Cohesion, Separation, Direction);
            SetBoidParams(boid);
            boids.Add(boid);
            // VARIABLE.onAlarmRaised += RaiseAlarm;
            // VARIABLE.onAlarmStop += StopAlarm;
        }
        
    }

    private void OnDisable()
    {
        foreach (var VARIABLE in agents)
        {
            // VARIABLE.onAlarmRaised -= RaiseAlarm;
            // VARIABLE.onAlarmStop -= StopAlarm;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            foreach (var VARIABLE in agents)
            {
                BoidAgent boid = VARIABLE.boid;
                SetBoidParams(boid);
            }
        }
    }

    private void SetBoidParams(BoidAgent boid)
    {
        boid.detectionRadious = detectionRadious;
        boid.aligmentWeight = aligmentWeight;
        boid.cohesionWeight = cohesionWeight;
        boid.separationWeight = separationWeight;
        boid.speed = speed;
    }

    public Vector3 Alignment(BoidAgent boid)
    {
        List<BoidAgent> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;
        foreach (BoidAgent b in insideRadiusBoids)
        {
            avg += b.parent.transform.right * b.speed;
        }

        avg /= insideRadiusBoids.Count;
        avg.Normalize();
        return avg;
    }

    public Vector3 Cohesion(BoidAgent boid)
    {
        List<BoidAgent> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;
        foreach (BoidAgent b in insideRadiusBoids)
        {
            avg += b.parent.transform.position;
        }

        avg /= insideRadiusBoids.Count;
        return (avg - boid.parent.transform.position).normalized;
    }

    public Vector3 Separation(BoidAgent boid)
    {
        List<BoidAgent> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;
        foreach (BoidAgent b in insideRadiusBoids)
        {
            avg += (b.parent.transform.position - boid.parent.transform.position);
        }

        avg /= insideRadiusBoids.Count;
        avg *= -1;
        avg.Normalize();
        return avg;
    }

    public Vector3 Direction(BoidAgent boid)
    {
        return (boid.objective - boid.parent.transform.position).normalized;
    }

    public List<BoidAgent> GetBoidsInsideRadius(BoidAgent boid)
    {
        List<BoidAgent> insideRadiusBoids = new List<BoidAgent>();

        foreach (BoidAgent b in boids)
        {
            float distance = Vector3.Distance(boid.parent.transform.position, b.parent.transform.position);
            if (distance < boid.detectionRadious)
            {
                insideRadiusBoids.Add(b);
            }
        }

        return insideRadiusBoids;
    }
}