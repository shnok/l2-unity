#if (UNITY_EDITOR) 
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


    private int flags = 0;
    public bool soulshot = false;
    public bool spiritshot = false;
    public bool crit = true;
    public int shld = 0;
    public bool miss = false;
    public int ssGrade = (int)EtcEffectInfo.EEP_GRADENONE;
    private int HITFLAG_USESS = 0x10;
    private int HITFLAG_CRIT = 0x20;
    private int HITFLAG_SHLD = 0x40;
    private int HITFLAG_MISS = 0x80;

    private IEnumerator DebugCoroutine()
    {
        yield return new WaitForSeconds(2f);
        // Skill ss = SkillTable.Instance.GetSkill(2039);
        // Skill sps = SkillTable.Instance.GetSkill(2047);

        while (true)
        {
            flags = 0;
            if (soulshot)
            {
                flags |= HITFLAG_USESS | ssGrade;
            }
            if (crit)
            {
                flags |= HITFLAG_CRIT;
            }
            if (shld > 0)
            {
                flags |= HITFLAG_SHLD;
            }
            if (miss)
            {
                flags |= HITFLAG_MISS;
            }
            // ParticleManager.Instance.SpawnSkillParticles(caster, ss);
            // ParticleManager.Instance.SpawnSkillParticles(target, sps);

            // ParticleManager.Instance.SpawnHitParticle(caster, target, true, true, (int)EtcEffectInfo.EEP_GRADENONE);
            // ParticleManager.Instance.SpawnHitParticle(target, caster, true, true, (int)EtcEffectInfo.EEP_GRADENONE);
            // ParticleManager.Instance.SpawnHitParticle(caster, target, false, true, (int)EtcEffectInfo.EEP_GRADENONE);

            Hit hit = new Hit(target.Identity.Id, 10, flags);
            Debug.Log($"Inflicting attack with flags: {flags} ss:{hit.hasSoulshot()} miss:{hit.isMiss()} crit:{hit.isCrit()}");
            WorldCombat.Instance.InflictAttack(caster, target, new Hit(target.Identity.Id, 10, flags));

            WorldCombat.Instance.EntityCastSkill(caster, spiritshot ? 2047 : 2039);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
#endif