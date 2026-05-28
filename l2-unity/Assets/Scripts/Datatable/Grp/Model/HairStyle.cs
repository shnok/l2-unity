using System.Collections.Generic;

public class HairStyle
{
    public string AhModel { get; set; }
    public string BhModel { get; set; }
    public string[] AhTextures { get; set; }
    public string[] BhTextures { get; set; }

    public override string ToString()
    {
        string ahTexturesFormatted = AhTextures != null ? string.Join(", ", AhTextures) : "None";
        string bhTexturesFormatted = BhTextures != null ? string.Join(", ", BhTextures) : "None";

        return $"HairStyle:\n" +
               $"  AhModel: {AhModel}\n" +
               $"  BhModel: {BhModel}\n" +
               $"  AhTextures: [{ahTexturesFormatted}]\n" +
               $"  BhTextures: [{bhTexturesFormatted}]";
    }
}