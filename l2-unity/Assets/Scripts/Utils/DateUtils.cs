using System;

public class DateUtils
{
    public static string ConvertSecondsToDate(int seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(Convert.ToDouble(seconds));

        if (t.Days > 0)
        {
            return $"{t.Days}Days {t.Hours}Hours {t.Minutes}Minutes";
        }

        if (t.Hours > 0)
        {
            return $"{t.Hours}Hours {t.Minutes}Minutes {t.Seconds}Seconds";
        }

        if (t.Minutes > 0)
        {
            return $"{t.Minutes}Minutes {t.Seconds}Seconds";
        }

        return $"{t.Seconds}Seconds";
    }
}