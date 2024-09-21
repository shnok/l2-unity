#if UNITY_EDITOR
public class SizeScale
{
    public float relativeTime;
    public float relativeSize;

    public override string ToString()
    {
        return $"RelativeTime={relativeTime} RelativeSize={relativeSize}";
    }
}
#endif