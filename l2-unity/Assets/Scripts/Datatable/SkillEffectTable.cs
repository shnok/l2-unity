public class SkillEffectTable
{
    private static SkillEffectTable _instance;
    public static SkillEffectTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillEffectTable();
            }

            return _instance;
        }
    }
}