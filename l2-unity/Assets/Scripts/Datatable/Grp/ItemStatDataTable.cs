using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public class ItemStatDataTable
{
    private static ItemStatDataTable _instance;
    public static ItemStatDataTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemStatDataTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, ItemStatData> _itemStatData;
    public Dictionary<int, ItemStatData> ItemsStatData { get { return _itemStatData; } }

    public void Initialize()
    {
        ReadItemStatDataDat();
    }

    public void ClearTable()
    {
        _itemStatData.Clear();
        _itemStatData = null;
        _instance = null;
    }

    private void ReadItemStatDataDat()
    {
        _itemStatData = new Dictionary<int, ItemStatData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/ItemStatData_Classic.txt");
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
                ItemStatData itemStatData = new ItemStatData();

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
                        case "object_id":
                            itemStatData.ObjectId = int.Parse(value);
                            break;
                        case "p_Defense":
                            itemStatData.PDef = int.Parse(value);
                            break;
                        case "m_Defense":
                            itemStatData.MDef = int.Parse(value);
                            break;
                        case "PAttack":
                            itemStatData.PAtk = int.Parse(value);
                            break;
                        case "mAttack":
                            itemStatData.MAtk = int.Parse(value);
                            break;
                        case "pAttackSpeed":
                            itemStatData.PAtkSpd = int.Parse(value);
                            break;
                        case "pHit":
                            itemStatData.PHit = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "mHit":
                            itemStatData.MHit = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "pCritical":
                            itemStatData.PCrit = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "mCritical":
                            itemStatData.MCrit = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "speed":
                            itemStatData.Speed = int.Parse(value);
                            break;
                        case "ShieldDefence":
                            itemStatData.ShieldDef = int.Parse(value);
                            break;
                        case "ShieldDefenceRate":
                            itemStatData.ShieldDefRate = int.Parse(value);
                            break;
                        case "pavoid":
                            itemStatData.PAvoid = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "mavoid":
                            itemStatData.MAvoid = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;

                    }
                }

                if (!ItemTable.Instance.ShouldLoadItem(itemStatData.ObjectId))
                {
                    continue;
                }

                _itemStatData.TryAdd(itemStatData.ObjectId, itemStatData);
            }

            Debug.Log($"Successfully imported {_itemStatData.Count} itemStatData(s)");
        }
    }

    public ItemStatData GetItemStatData(int id)
    {
        ItemStatData itemStatData;
        _itemStatData.TryGetValue(id, out itemStatData);
        return itemStatData;
    }
}
