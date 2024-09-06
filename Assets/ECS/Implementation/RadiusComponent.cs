using System.Collections.Generic;

public class RadiusComponent : ECSComponent
{
    public float radius;

    public RadiusComponent(List<Boid> nearBoids, float radius)
    {
     
        this.radius = radius;
    }
}