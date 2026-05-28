using System;

public class StatsConverter
{
    private static StatsConverter _instance;
    public static StatsConverter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StatsConverter();
            }
            return _instance;
        }
    }

    public float ConvertStat(Stat statType, int value)
    {
        switch (statType)
        {
            case Stat.SPEED:
                return ConvertSpeed(value);
            case Stat.PHYS_ATTACK_SPEED:
                return ConvertPAtkSpd(value);
            case Stat.MAGIC_ATTACK_SPEED:
                return ConvetMAtkSpd(value);
        }

        return value;
    }

    private float ConvertSpeed(int value)
    {
        return NumberUtils.ScaleToUnity(value);
    }

    private float ConvertPAtkSpd(int value)
    {
        if (value < 2)
            return 2700;

        return Math.Max(100, 500000 / value);
    }

    private float ConvetMAtkSpd(int value)
    {
        //        if (skill.isMagic()) {
        //            return (int) ((skillTime * 333) / attacker.getMAtkSpd());
        //        }
        //        return (int) ((skillTime * 333) / attacker.getPAtkSpd());
        return value;
    }
}
