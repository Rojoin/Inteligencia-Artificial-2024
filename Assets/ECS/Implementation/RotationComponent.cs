public class RotationComponent : ECSComponent
{
    public float X;
    public float Y;
    public float Z;
    public float w;

    public RotationComponent(float X, float Y, float Z,float w) 
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.w = w;
    }
}