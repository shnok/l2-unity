using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SysStringTable
{
    private static SysStringTable _instance;
    private Dictionary<int, SysStringData> _strings;

    public static SysStringTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SysStringTable();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        ReadFile();
    }

    private void ReadFile()
    {
        _strings = new Dictionary<int, SysStringData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SysString_Classic-eu.txt");
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
                SysStringData sysData = new SysStringData();

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
                        case "stringID":
                            sysData.Id = int.Parse(value);
                            break;
                        case "string":
                            sysData.Name = DatUtils.CleanupString(value);
                            break;
                    }
                }

                if (_strings.ContainsKey(sysData.Id) != true)
                {
                    _strings.TryAdd(sysData.Id, sysData);
                }
            }

            Debug.Log($"Successfully imported {_strings.Count} SysString_Classic(s)");
        }
    }

    public SysStringData GetSysString(int id)
    {
        if (_strings.TryGetValue(id, out SysStringData data))
        {
            return data;
        }
        else
        {
            Debug.LogWarning($"SysString not found for id [{id}]");
            return null;
        }
    }
}
