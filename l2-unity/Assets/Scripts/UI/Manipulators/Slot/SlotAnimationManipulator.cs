using UnityEngine;
using UnityEngine.UIElements;

public class SlotAnimationManipulator : Manipulator
{
    private VisualElement _root;
    private SkillSlot _slot;
    public SlotAnimationManipulator(VisualElement target, SkillSlot skillSlot)
    {
        _root = target;
        this.target = target;
        _slot = skillSlot;
    }

    protected override void RegisterCallbacksOnTarget()
    {
    }

    protected override void UnregisterCallbacksFromTarget()
    {
    }

    public bool AnimateCoolTime(float now)
    {
        if (now >= _slot.SkillInfo.CooldownEndTime)
        {
            if (now >= _slot.SkillInfo.CooldownEndTime + 0.25f)
            {
                if (now < _slot.SkillInfo.CooldownEndTime + 0.5f) // buffer to avoid updating the background forever
                {
                    _slot.SlotEffect.style.backgroundImage = new StyleBackground();
                }
                return true;
            }
            else
            {
                float elapsedSinceCooldownEnd = now - _slot.SkillInfo.CooldownEndTime;
                float skillEndRatio = elapsedSinceCooldownEnd / 0.25f;
                int skillEndRatioIndex = Mathf.Clamp((int)Mathf.Round(skillEndRatio * SkillbarImageTable.Instance.CooltimeEndTextures.Length - 1), 0, SkillbarImageTable.Instance.CooltimeEndTextures.Length - 1);
                _slot.SlotEffect.style.backgroundImage = new StyleBackground(SkillbarImageTable.Instance.CooltimeEndTextures[skillEndRatioIndex]);
            }
        }
        else
        {
            float elapsed = now - _slot.SkillInfo.CooldownStartTime;
            float duration = _slot.SkillInfo.CooldownEndTime - _slot.SkillInfo.CooldownStartTime;
            float ratio = elapsed / duration;
            int imageIndex = Mathf.Clamp((int)Mathf.Round(ratio * SkillbarImageTable.Instance.CooltimeTextures.Length - 1), 0, SkillbarImageTable.Instance.CooltimeTextures.Length - 1);

            _slot.SlotEffect.style.backgroundImage = new StyleBackground(SkillbarImageTable.Instance.CooltimeTextures[imageIndex]);
        }

        return false;
    }

    public void AnimateToggle(float now)
    {
        int index = (int)(now * 10f % 12);
        _slot.SlotEffect.style.backgroundImage = new StyleBackground(SkillbarImageTable.Instance.ToggleTextures[index]);
    }
}