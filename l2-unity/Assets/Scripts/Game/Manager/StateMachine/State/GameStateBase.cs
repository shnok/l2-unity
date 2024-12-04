public abstract class GameStateBase
{
    protected bool _uiLoaded = false;
    protected bool _worldLoaded = false;
    protected GameManager _stateMachine;

    public GameStateBase(GameManager stateMachine)
    {
        _uiLoaded = false;
        _worldLoaded = false;

        _stateMachine = stateMachine;
    }

    public virtual void Enter(object arg0) { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void HandleEvent(GameEvent evt, object arg0)
    {
        if (evt == GameEvent.UI_LOADED)
        {
            _uiLoaded = true;
        }
        else if (evt == GameEvent.WORLD_LOADED)
        {
            _worldLoaded = true;
        }

        if (_uiLoaded && _worldLoaded)
        {
            _stateMachine.NotifyEvent(GameEvent.LOADING_COMPLETE);
        }
    }
}