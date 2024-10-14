using System.Collections;
using UnityEngine;

public class SkillEffectTest : MonoBehaviour
{

    public Entity caster;
    public Entity target;
    public float spawnDelay = 1f;

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
        yield return new WaitForSeconds(2f);
        // Skill ss = SkillTable.Instance.GetSkill(2039);
        // Skill sps = SkillTable.Instance.GetSkill(2047);
        while (true)
        {
            // ParticleManager.Instance.SpawnSkillParticles(caster, ss);
            // ParticleManager.Instance.SpawnSkillParticles(target, sps);

            // ParticleManager.Instance.SpawnHitParticle(caster, target, true, true, (int)EtcEffectInfo.EEP_GRADENONE);
            ParticleManager.Instance.SpawnHitParticle(target, caster, true, true, (int)EtcEffectInfo.EEP_GRADENONE);
            // ParticleManager.Instance.SpawnHitParticle(caster, target, false, true, (int)EtcEffectInfo.EEP_GRADENONE);
            //  SpawnSkillCast()
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}