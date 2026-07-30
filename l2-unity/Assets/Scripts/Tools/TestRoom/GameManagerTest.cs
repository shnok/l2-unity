#if (UNITY_EDITOR) 
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private LayerMask _clickThroughMask;

    void Awake()
    {
        LoadTables();
        // ClickManager.Instance.SetMasks(_entityMask, _clickThroughMask);
    }

    private void LoadTables()
    {
        SkillSoundgrpTable.Instance.Initialize();
        SkillTable.Instance.Initialize();
        ItemTable.Instance.Initialize();
        ItemNameTable.Instance.Initialize();
        ItemStatDataTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        WeapongrpTable.Instance.Initialize();
        NpcgrpTable.Instance.Initialize();
        NpcNameTable.Instance.Initialize();
        ActionNameTable.Instance.Initialize();
        SysStringTable.Instance.Initialize();
        SkillNameTable.Instance.Initialize();
        SkillgrpTable.Instance.Initialize();
        LogongrpTable.Instance.Initialize();
        SystemMessageTable.Instance.Initialize();
        IconTable.Instance.Initialize();
        SkillbarImageTable.Instance.Initialize();

        // // Caching
        ItemTable.Instance.CacheItems();
        SkillTable.Instance.CacheSkills();
        ChargrpTable.Instance.Initialize();
        ModelTable.Instance.Initialize();
        SkillEffectTable.Instance.Initialize();
        ParticleEffectTable.Instance.Initialize();

        // // Memory cleanup
        ArmorgrpTable.Instance.ClearTable();
        EtcItemgrpTable.Instance.ClearTable();
        ItemNameTable.Instance.ClearTable();
        SkillgrpTable.Instance.ClearTable();
        SkillNameTable.Instance.ClearTable();
    }

}
#endif