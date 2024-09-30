using System.Collections;
using UnityEngine;

public class SkillEffectTest : MonoBehaviour
{

    public Entity caster;
    public Entity target;

    void Awake()
    {
        caster = GameObject.Find("Caster").GetComponent<Entity>();
        caster.Initialize();
        caster.GetComponent<Gear>().Initialize(0, CharacterRaceAnimation.FDarkElf);
        target = GameObject.Find("Target").GetComponent<Entity>();
        target.Initialize();
        target.GetComponent<Gear>().Initialize(1, CharacterRaceAnimation.FDarkElf);
    }

    void Start()
    {
        StartCoroutine(DebugCoroutine());
    }

    private IEnumerator DebugCoroutine()
    {
        Skill skill = SkillTable.Instance.GetSkill(2039);
        Debug.Log(skill);
        while (true)
        {
            ParticleManager.Instance.CastSkill(caster, skill);
            //  SpawnSkillCast()
            yield return new WaitForSeconds(1);
        }
    }
}