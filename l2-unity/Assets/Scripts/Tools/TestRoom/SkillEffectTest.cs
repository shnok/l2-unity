using System.Collections;
using UnityEngine;

public class SkillEffectTest : MonoBehaviour
{

    public Entity caster;
    public Entity target;

    void Awake()
    {
        if (caster == null)
        {
            caster = GameObject.Find("Caster").GetComponent<Entity>();
            //caster.Initialize();
            caster.GetComponent<Gear>().Initialize(0, CharacterModelType.FDarkElf);
        }
        if (target == null)
        {
            target = GameObject.Find("Caster").GetComponent<Entity>();
            //caster.Initialize();
            target.GetComponent<Gear>().Initialize(0, CharacterModelType.FDarkElf);
        }
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
            ParticleManager.Instance.SpawnSkillParticles(caster, skill);
            //  SpawnSkillCast()
            yield return new WaitForSeconds(1);
        }
    }
}