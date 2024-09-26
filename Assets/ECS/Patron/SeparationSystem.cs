using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SeparationSystem : ECSSystem
{
    private ParallelOptions parallelOptions;
    private IDictionary<uint, PositionComponent> positionComponents;
    private IDictionary<uint, RadiusComponent> radiusComponents;
    private IDictionary<uint, SeparationComponent> separationComponents;
    private IEnumerable<uint> queryedEntities;
    private IDictionary<uint, List<uint>> nearBoids;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        radiusComponents ??= ECSManager.GetComponents<RadiusComponent>();
        separationComponents ??= ECSManager.GetComponents<SeparationComponent>();
        positionComponents ??= ECSManager.GetComponents<PositionComponent>();
        queryedEntities ??=
            ECSManager.GetEntitiesWhitComponentTypes(typeof(RadiusComponent), typeof(PositionComponent),
                typeof(SeparationComponent));


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
                separationComponents[i].X += positionComponents[i].X - positionComponents[j.Key].X;
                separationComponents[i].Y += positionComponents[i].Y - positionComponents[j.Key].X;
                separationComponents[i].Z += positionComponents[i].Z - positionComponents[j.Key].X;
            });


            avg /= nearBoids.Count;

            avg.Normalize();
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
}