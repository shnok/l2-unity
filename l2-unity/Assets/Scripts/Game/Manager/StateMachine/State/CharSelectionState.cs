using UnityEngine;

public class CharSelectionState : GameStateBase
{
    public CharSelectionState(GameManager stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        RefreshCharSelection();
    }

    public override void Update()
    {

    }

    public override void HandleEvent(GameEvent evt, object arg0)
    {
        switch (evt)
        {
            case GameEvent.GAME_DISCONNECTED:
                L2LoginUI.Instance.ShowLoginWindow();
                _stateMachine.ChangeState(GameState.LOGIN_SCREEN);
                break;
            case GameEvent.CHAR_SELECTED:
                _stateMachine.ChangeState(GameState.ENTERING_WORLD);
                break;
            case GameEvent.CHAR_LOADED:
                RefreshCharSelection();
                break;
            default:
                Debug.LogWarning($"[GameStateMachine] Unhandled event {evt} for state {_stateMachine.State}");
                break;
        }
    }

    private void RefreshCharSelection()
    {
        L2LoginUI.Instance.ShowCharSelectWindow();

        CharSelectWindow.Instance.SetCharacterList(CharacterSelector.Instance.Characters);

        CharacterSelector.Instance.ApplyCharacterList();
        CharacterSelector.Instance.SelectDefaultCharacter();

        CharSelectWindow.Instance.SelectSlot(CharacterSelector.Instance.SelectedSlot);

        LoginCameraManager.Instance.SwitchCamera("CharSelect");
    }
}