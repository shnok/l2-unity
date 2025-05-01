using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BuffWindow : AbstractEffectWindow
{
    private static BuffWindow _instance;
    public static BuffWindow Instance { get { return _instance; } }

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
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/BuffWindow/BuffWindow");
        _buffSlot = LoadAsset("Data/UI/_Elements/Game/BuffWindow/BuffSlot");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        _effectType = EffectType.Buff;

        base.Init(root);

        _buffRowContainers = new VisualElement[3];
        _buffRowContainers[0] = _windowEle.Q<VisualElement>("NormalBuffs");
        _buffRowContainers[1] = _windowEle.Q<VisualElement>("SpecialBuffs");
        _buffRowContainers[2] = _windowEle.Q<VisualElement>("ToggleBuffs");

        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
    }

    public override void SetEtcEffects(PlayerBuffStatus status)
    {
        // to fix: skillid - duelist sonic focus = 8 or tyrant focus force = 50 
        if (status.Charges > 0) AddEffect(8, status.Charges, 600, BuffType.Special);
    }
}
