﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AligmentSystem : ECSSystem
{
    private ParallelOptions parallelOptions;

    private IDictionary<uint, PositionComponent> positionComponents;
    private IDictionary<uint, FowardComponent> fowardComponents;
    private IDictionary<uint, RadiusComponent> radiusComponents;
    private IDictionary<uint, SpeedComponent> speedComponents;
    private IDictionary<uint, AlignmentComponent> alignmentComponents;
    private IEnumerable<uint> queryedEntities;
    private IDictionary<uint, List<uint>> nearBoids;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        radiusComponents ??= ECSManager.GetComponents<RadiusComponent>();
        fowardComponents ??= ECSManager.GetComponents<FowardComponent>();
        alignmentComponents ??= ECSManager.GetComponents<AlignmentComponent>();
        positionComponents ??= ECSManager.GetComponents<PositionComponent>();
        queryedEntities ??=
            ECSManager.GetEntitiesWhitComponentTypes(typeof(RadiusComponent), typeof(PositionComponent),typeof(AlignmentComponent));
        AddRadius();
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            
            Vector3 avg = Vector3.zero;

            Parallel.ForEach(nearBoids, parallelOptions, j =>
            {
                alignmentComponents[i].X += fowardComponents[j.Key].X * speedComponents[j.Key].X;
                alignmentComponents[i].Y += fowardComponents[j.Key].Y * speedComponents[j.Key].X;
                alignmentComponents[i].Z += fowardComponents[j.Key].Z * speedComponents[j.Key].X;
            });
            
            
            avg /= nearBoids.Count;
            
            avg.Normalize();
        });
    }

    protected override void PostExecute(float deltaTime)
    {
  
    }

    private void AddRadius()
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            List<uint> insideRadiusBoids = new List<uint>();

            Parallel.ForEach(queryedEntities, parallelOptions, j =>
            {
                if (positionComponents[i] != positionComponents[j])
                {
                    float distance = Mathf.Abs(positionComponents[i].X - positionComponents[j].X) +
                                     Mathf.Abs(positionComponents[i].Y - positionComponents[i].Y) +
                                     Mathf.Abs(positionComponents[i].Z - positionComponents[i].Z);
                    if (distance < radiusComponents[i].radius)
                    {
                        insideRadiusBoids.Add(j);
                    }
                }
            });


            nearBoids[i] = insideRadiusBoids;
        });
    }
}