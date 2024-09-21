#if UNITY_EDITOR
public class Range3D
{
    public Range x;
    public Range y;
    public Range z;

    public Range3D()
    {
        x = new Range();
        y = new Range();
        z = new Range();
    }

    public override string ToString()
    {
        return $"X={x.ToString()} Y={y.ToString()} Z={z.ToString()}";
    }
}
#endif