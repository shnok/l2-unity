using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillNameTable : MonoBehaviour
{
    private static SkillNameTable _instance;

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

    private Dictionary<int, Dictionary<int, SkillNameData>> _names;

    //public SkillNameData GetName(int id)
    //{
    //    return (_names.ContainsKey(id)) ? _names[id] : null;
    //}
    public Dictionary<int, Dictionary<int, SkillNameData>> Names { get { return _names; } }

    public void Initialize()
    {
        ReadActions();
    }

    private void ReadActions()
    {
        _names = new Dictionary<int, Dictionary<int, SkillNameData>>();
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
