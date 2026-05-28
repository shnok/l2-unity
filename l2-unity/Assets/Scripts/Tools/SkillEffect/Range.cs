#if UNITY_EDITOR
public class Range
{
    public float min;
    public float max;

    public override string ToString()
    {
        return $"Min={min} Max={max}";
    }
}
#endif