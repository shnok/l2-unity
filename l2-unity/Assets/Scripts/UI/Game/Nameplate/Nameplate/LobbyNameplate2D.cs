using UnityEngine;
using UnityEngine.UIElements;

public class LobbyNameplate2D : Nameplate2D
{
    private bool _isDeleteTimerVisible = false;

    private Label _deleteTimerLabel;

    public LobbyNameplate2D(VisualElement visualElement, Label entityName, Label entityTitle, Entity entity) : base(visualElement, entityName, entityTitle, entity)
    {
        _deleteTimerLabel = visualElement.Q<Label>("DeleteTimer");
    }

    public void ShowDeleteTimer()
    {

        if (!_isDeleteTimerVisible)
        {
            _isDeleteTimerVisible = true;
            SetClassName(_nameplateEle, "delete-timer");
        }
    }

    public void HideDeleteTimer()
    {
        if (_isDeleteTimerVisible)
        {
            _isDeleteTimerVisible = false;
            RemoveClassName(_nameplateEle, "delete-timer");
        }
    }

    public void UpdateTimer(int deleteTImer)
    {
        _deleteTimerLabel.text = $"Remaining Time : {DateUtils.ConvertSecondsToDate(deleteTImer)}";
    }
}
