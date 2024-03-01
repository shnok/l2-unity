using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EtcItemgrpTable {
    private static EtcItemgrpTable _instance;
    public static EtcItemgrpTable Instance {
        get {
            if (_instance == null) {
                _instance = new EtcItemgrpTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, EtcItemgrp> _etcItemGrps;
    public Dictionary<int, EtcItemgrp> EtcItemGrps { get { return _etcItemGrps; } }

    public void Initialize() {
        ReadEtcItemgrpDat();
    }

    private void ReadEtcItemgrpDat() {
        _etcItemGrps = new Dictionary<int, EtcItemgrp>();
        string dataPath = "Assets/Resources/Data/Meta/EtcItemgrp_Classic.txt";
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                EtcItemgrp etcItemgrp = new EtcItemgrp();
                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    if (DatUtils.ParseBaseAbstractGrpDat(etcItemgrp, key, value)) {
                        continue;
                    }

                    switch (key) {
                        case "etcitem_type": 
                            etcItemgrp.EtcItemType = value;
                            break;                
                    }
                }

                if (!ItemTable.Instance.ShouldLoadItem(etcItemgrp.ObjectId) || _etcItemGrps.ContainsKey(etcItemgrp.ObjectId)) {
                    continue;
                }

                _etcItemGrps.Add(etcItemgrp.ObjectId, etcItemgrp);
            }

            Debug.Log($"Successfully imported {_etcItemGrps.Count} etcItemgrp(s)");
        }
    }
}