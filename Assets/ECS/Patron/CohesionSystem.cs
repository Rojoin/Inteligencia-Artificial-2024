using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CohesionSystem : ECSSystem
{    private ParallelOptions parallelOptions;

    private IDictionary<uint, PositionComponent> positionComponents;
    private IDictionary<uint, RadiusComponent> radiusComponents;
    private IDictionary<uint, CohesionComponent> cohesionComponents;
    private IEnumerable<uint> queryedEntities;
    private IDictionary<uint, List<uint>> nearBoids;
    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        radiusComponents ??= ECSManager.GetComponents<RadiusComponent>();
        cohesionComponents ??= ECSManager.GetComponents<CohesionComponent>();
        positionComponents ??= ECSManager.GetComponents<PositionComponent>();
        queryedEntities ??=
            ECSManager.GetEntitiesWhitComponentTypes(typeof(RadiusComponent), typeof(PositionComponent),typeof(AlignmentComponent));
        
        
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

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            Vector3 avg = Vector3.zero;
            Parallel.ForEach(nearBoids, parallelOptions, j =>
            {
                cohesionComponents[i].X += positionComponents[j.Key].X;
                cohesionComponents[i].Y += positionComponents[j.Key].X;
                cohesionComponents[i].Z += positionComponents[j.Key].X;
            });
            
            
            avg /= nearBoids.Count;
            
            avg.Normalize();
        });
    }

    protected override void PostExecute(float deltaTime)
    {
   
    }
}