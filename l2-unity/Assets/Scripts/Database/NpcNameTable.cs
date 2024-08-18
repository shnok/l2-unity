using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NpcNameTable {
    private static NpcNameTable _instance;
    public static NpcNameTable Instance {
        get {
            if (_instance == null) {
                _instance = new NpcNameTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, NpcName> _npcNames;

    public Dictionary<int, NpcName> NpcNames { get { return _npcNames; } }

    public void Initialize() {
        ReadNpcgrps();
    }

    private void ReadNpcgrps() {
        _npcNames = new Dictionary<int, NpcName>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/NpcName_Classic-eu.txt");
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                NpcName npcName = new NpcName();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    switch (key) {
                        case "id":
                            npcName.Id = int.Parse(value);
                            break;
                        case "name":
                            npcName.Name = DatUtils.CleanupString(value);
                            break;
                        case "nick":
                            npcName.Title = DatUtils.CleanupString(value);
                            break;
                        case "nickcolor":
                            npcName.TitleColor = DatUtils.CleanupString(value);
                            break;                      
                    }
                }

                _npcNames.TryAdd(npcName.Id, npcName);
            }

            Debug.Log($"Successfully imported {_npcNames.Count} npcName(s)");
        }
    }

    public NpcName GetNpcName(int id) {
        NpcName npcName = null;
        _npcNames.TryGetValue(id, out npcName);

        if (npcName == null) {
            Debug.LogWarning($"NpcName not found for id [{id}]");
        }

        return npcName;
    }
}