using UnityEngine;

public class ColorUtils
{
    public static Color HexToColor(string hex)
    {
        // Remove the alpha channel from the string (last two characters)
        hex = hex.Substring(0, 6);

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        // Convert the color values to the range [0, 1]
        float rf = r / 255f;
        float gf = g / 255f;
        float bf = b / 255f;

        return new Color(rf, gf, bf, 1f);
    }

    public static int HexToInteger(string hex)
    {
        // Remove the alpha channel from the string (last two characters)
        hex = hex.Substring(0, 6);

        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return (r & 0xFF) + ((g & 0xFF) << 8) + ((b & 0xFF) << 16);
    }

    public static Color IntegerToColor(int value)
    {
        // Extract the RGB components from the integer
        int r = value & 0xFF;
        int g = (value >> 8) & 0xFF;
        int b = (value >> 16) & 0xFF;

        // Convert to float and create a Unity Color
        float rf = r / 255f;
        float gf = g / 255f;
        float bf = b / 255f;

        return new Color(rf, gf, bf, 1f);
    }
}