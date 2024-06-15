#if (UNITY_EDITOR) 
[System.Serializable]
public class Poly {
    public string name;
    public int polyCount;
    public PolyData[] polyData;
    public override string ToString() {
        string polyDataString = "";
        foreach (var poly in polyData) {
            polyDataString = poly.ToString() + " ";
        }
        return $"Name: {name}, polyCount: {polyCount}, polyData: {polyDataString}";
    }
}
#endif