using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private Dictionary<ActionType, L2Action> _actions;
    private static PlayerActions _instance;
    public static PlayerActions Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _actions = new Dictionary<ActionType, L2Action>();
        _actions.Add(ActionType.Attack, new AttackAction());
        _actions.Add(ActionType.Sit, new SitStandAction());
        _actions.Add(ActionType.WalkRun, new WalkRunAction());
        _actions.Add(ActionType.NextTarget, new NextTargetAction());
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Update()
    {
        ListenToKeybindedActions();
    }

    private void ListenToKeybindedActions()
    {
        if (InputManager.Instance.Attack)
        {
            UseAction(ActionType.Attack);
        }

        if (InputManager.Instance.NextTarget)
        {
            UseAction(ActionType.NextTarget);
        }
    }

    public void UseAction(ActionType actionType)
    {
        if (_actions.TryGetValue(actionType, out L2Action action))
        {
            action.UseAction();
        }
        else
        {
            Debug.LogWarning("Action not found.");
        }
    }
}