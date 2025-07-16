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
        _effectType = EffectType.Debuff;

        base.Init(root);

        _buffRowContainers = new VisualElement[3];
        _buffRowContainers[0] = _windowEle.Q<VisualElement>("NormalBuffs");
        _buffRowContainers[1] = _windowEle.Q<VisualElement>("SpecialBuffs");
        _buffRowContainers[2] = _windowEle.Q<VisualElement>("ToggleBuffs");

        _windowEle.style.top = 40;

        yield return new WaitForEndOfFrame();

        L2GameUI.Instance.WindowLoadComplete();
    }

    public override void SetEtcEffects(PlayerBuffStatus status)
    {
        base.SetEtcEffects(status);

        // treat it as debuff
        if (status.DeathPenaltyLvl > 0) AddEffect(5076, status.DeathPenaltyLvl, -100, BuffType.Special);
        if (status.WeightPenalty > 0) AddEffect(4270, status.WeightPenalty, -100, BuffType.Special);
        if (status.IsInsideDangerZone) AddEffect(4268, 1, -100, BuffType.Special);
        if (status.IsBlockingAllPlayers) AddEffect(4269, 1, -100, BuffType.Special);
        if (status.HasCharmOfCourage) AddEffect(5041, 1, -100, BuffType.Special);
        if (status.HasGradePenalty) AddEffect(4267, 1, -100, BuffType.Special); //Calvaz: needs fix: 6209 for armor, 6213 for weapon
                                                                                //Shnok: It looks like interlude doesnt have different level of weight penalty, the effect id is 4267

        HideWindowIfNeeded();
    }
}
