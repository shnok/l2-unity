#if UNITY_EDITOR
public class ColorScale
{
    public float relativeTime;
    public int r;
    public int g;
    public int b;
    public int a;

    public override string ToString()
    {
        return $"RelativeTime={relativeTime} R={r} G={g} B={b} A={a}";
    }
}
#endif