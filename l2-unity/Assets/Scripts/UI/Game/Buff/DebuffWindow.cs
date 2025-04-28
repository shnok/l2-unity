using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DebuffWindow : AbstractEffectWindow
{
    [SerializeField] private int _debuffEffectsCount;
    
    private static DebuffWindow _instance;
    public static DebuffWindow Instance { get { return _instance; } }
    
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
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/BuffWindow/DebuffWindow");
        _buffSlot = LoadAsset("Data/UI/_Elements/Game/BuffWindow/DebuffSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        _buffRows = new int[1];
        _buffRows[0] = _debuffEffectsCount;
        _effectType = EffectType.Debuff;
        
        base.Init(root);
        
        _buffRowContainers = new VisualElement[1];
        _buffRowContainers[0] = _windowEle.Q<VisualElement>("DebuffBuffs");
        _windowEle.style.top = 40;
        
        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
    }

    protected override void TogglePulse(VisualElement element)
    {
        if (element.ClassListContains("fade-low-red"))
        {
            element.RemoveFromClassList("fade-low-red");
            element.AddToClassList("fade-high-red");
        }
        else
        {
            element.RemoveFromClassList("fade-high-red");
            element.AddToClassList("fade-low-red");
        }
    }
}
