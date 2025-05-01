using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class BuffWindow : AbstractEffectWindow
{
    [SerializeField] private int _normalBuffsCount;
    [SerializeField] private int _specialBuffsCount;
    [SerializeField] private int _toggleBuffsCount;
    
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
        _buffRows = new int[3];
        _buffRows[0] = _normalBuffsCount;
        _buffRows[1] = _specialBuffsCount;
        _buffRows[2] = _toggleBuffsCount;
        _effectType = EffectType.Buff;
        
        base.Init(root);
        
        _buffRowContainers = new VisualElement[3];
        _buffRowContainers[0] = _windowEle.Q<VisualElement>("NormalBuffs");
        _buffRowContainers[1] = _windowEle.Q<VisualElement>("SpecialBuffs");
        _buffRowContainers[2] = _windowEle.Q<VisualElement>("ToggleBuffs");
        
        yield return new WaitForEndOfFrame();
        
        L2GameUI.Instance.WindowLoadComplete();
    }

    public void UpsertPlayerStatus(PlayerBuffStatus status)
    {
        // to fix: skillid - duelist sonic focus = 8 or tyrant focus force = 50 
        if (status.Charges > 0) UpsertEffect(8, status.Charges, 600, BuffType.Special);

        // treat it as debuff
        if (status.IsInsideDangerZone) UpsertEffect(4268, 1, -100, BuffType.Debuff);
        if (status.IsBlockingAllPlayers) UpsertEffect(4269, 1, -100, BuffType.Special);
        if (status.WeightPenalty > 0) UpsertEffect(4270, status.WeightPenalty, -50, BuffType.Special);
        if (status.HasCharmOfCourage) UpsertEffect(5041, 1, -100, BuffType.Special);
        if (status.DeathPenaltyLvl > 0) // needs fix
        if (status.HasGradePenalty) UpsertEffect(6209, 1, -100, BuffType.Debuff); //needs fix: 6209 for armor, 6213 for weapon
    }

    protected override void TogglePulse(VisualElement element)
    {
        if (element.ClassListContains("fade-low"))
        {
            element.RemoveFromClassList("fade-low");
            element.AddToClassList("fade-high");
        }
        else
        {
            element.RemoveFromClassList("fade-high");
            element.AddToClassList("fade-low");
        }
    }
}
