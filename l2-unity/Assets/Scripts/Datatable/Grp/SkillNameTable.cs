using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillNameTable
{
    private static SkillNameTable _instance;
    private static Dictionary<int, Dictionary<int, SkillNameData>> _names;
    public Dictionary<int, Dictionary<int, SkillNameData>> Names { get { return _names; } }
    public static SkillNameTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillNameTable();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        _names = new Dictionary<int, Dictionary<int, SkillNameData>>();
        ReadActions();
    }

    public void ClearTable()
    {
        _names.Clear();
        _names = null;
        _instance = null;
    }

    public SkillNameData GetName(int id, int level)
    {
        _names.TryGetValue(id, out Dictionary<int, SkillNameData> skillLevel);

        if (skillLevel == null) return null;

        skillLevel.TryGetValue(level, out SkillNameData skillName);

        return skillName;
    }

    public Dictionary<int, SkillNameData> GetNames(int id)
    {
        _names.TryGetValue(id, out Dictionary<int, SkillNameData> skillLevel);

        return skillLevel;
    }

    private void ReadActions()
    {

        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SkillName_Classic-eu.txt");

        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                SkillNameData nameData = new SkillNameData();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++)
                {
                    if (!keyvals[i].Contains("="))
                    {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    switch (key)
                    {
                        case "skill_id":
                            nameData.Id = int.Parse(value);
                            break;
                        case "skill_level":
                            nameData.Level = int.Parse(value);
                            break;
                        case "skill_sublevel":
                            nameData.SubLevel = int.Parse(value);
                            break;
                        case "name":
                            nameData.Name = DatUtils.CleanupString(value);
                            break;
                        case "desc":
                            nameData.Desc = DatUtils.CleanupString(value);
                            break;
                        case "prev_skill_id":
                            nameData.PrevSkillId = int.Parse(value);
                            break;
                    }
                }

                if (!SkillTable.Instance.ShouldLoadSkill(nameData.Id))
                {
                    continue;
                }

                TryAdd(nameData);
            }

            Debug.Log($"Successfully imported {_names.Count} actionName(s)");
        }
    }

    private void TryAdd(SkillNameData skillName)
    {

        if (_names.ContainsKey(skillName.Id) == true)
        {
            Dictionary<int, SkillNameData> dataGrp = _names[skillName.Id];
            dataGrp.TryAdd(skillName.Level, skillName);
        }
        else
        {
            Dictionary<int, SkillNameData> dataGrp = CreateDict();
            AddDict(dataGrp, skillName);
            _names.TryAdd(skillName.Id, dataGrp);
        }
    }

    private Dictionary<int, SkillNameData> CreateDict()
    {
        return new Dictionary<int, SkillNameData>();
    }

    private void AddDict(Dictionary<int, SkillNameData> dataGrp, SkillNameData skillName)
    {
        dataGrp.TryAdd(skillName.Level, skillName);
    }
}
