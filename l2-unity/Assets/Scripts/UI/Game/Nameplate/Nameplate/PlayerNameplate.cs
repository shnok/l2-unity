using UnityEngine;
using UnityEngine.UIElements;

public class PlayerNameplate : Nameplate
{
    private bool _isGaugeVisible = false;
    public float GaugeStartTime { get; private set; }
    public float GaugeEndTime { get; private set; }
    public const float GAUGE_WIDTH = 80f;

    private VisualElement _gauge;

    public PlayerNameplate(VisualElement visualElement, Label entityName, Label entityTitle, Entity entity) : base(visualElement, entityName, entityTitle, entity)
    {
        _gauge = visualElement.Q<VisualElement>("Gauge");
    }

    public void ShowGauge(SetupGaugePacket.GaugeColor color, float startTime, int durationMs)
    {
        GaugeStartTime = startTime;
        GaugeEndTime = startTime + durationMs / 1000f;

        if (!_isGaugeVisible)
        {
            _isGaugeVisible = true;
            SetClassName(_nameplateEle, "gauge");
        }
    }

    public void HideGauge()
    {
        if (_isGaugeVisible)
        {
            _isGaugeVisible = false;
            RemoveClassName(_nameplateEle, "gauge");
        }
    }

    public void UpdateGauge(float currentTime)
    {
        if (_isGaugeVisible)
        {
            float gaugeRatio = (currentTime - GaugeStartTime) / (GaugeEndTime - GaugeStartTime);
            // Debug.LogWarning($"currentTime: {currentTime} GaugeStartTime: {GaugeStartTime} GaugeEndTime: {GaugeEndTime} Ratio: {gaugeRatio}");
            gaugeRatio = Mathf.Clamp(gaugeRatio, 0f, 1f);
            _gauge.style.width = gaugeRatio * GAUGE_WIDTH;
        }
    }
}
