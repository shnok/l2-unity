using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ItemNameTable
{
    private static ItemNameTable _instance;
    public static ItemNameTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemNameTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, ItemName> _itemNames;
    public Dictionary<int, ItemName> ItemNames { get { return _itemNames; } }

    public void Initialize()
    {
        ReadItemNameDat();
    }

    public void ClearTable()
    {
        _itemNames.Clear();
        _itemNames = null;
        _instance = null;
    }

    private void ReadItemNameDat()
    {
        _itemNames = new Dictionary<int, ItemName>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/ItemName_Classic-eu.txt");
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
                ItemName itemName = new ItemName();

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
                        case "id":
                            itemName.Id = int.Parse(value);
                            break;
                        case "name":
                            itemName.Name = DatUtils.CleanupString(value);
                            break;
                        case "description":
                            itemName.Description = DatUtils.CleanupString(value);
                            break;
                        case "default_action":
                            itemName.DefaultAction = DatUtils.CleanupString(value);
                            break;
                        case "is_trade":
                            itemName.Tradeable = value == "1";
                            break;
                        case "is_drop":
                            itemName.Droppable = value == "1";
                            break;
                        case "is_destruct":
                            itemName.Destructible = value == "1";
                            break;
                        case "is_npctrade":
                            itemName.Sellable = value == "1";
                            break;
                    }
                }

                if (!ItemTable.Instance.ShouldLoadItem(itemName.Id))
                {
                    continue;
                }

                _itemNames.TryAdd(itemName.Id, itemName);
            }

            Debug.Log($"Successfully imported {_itemNames.Count} itemNames(s)");
        }
    }

    public ItemName GetItemName(int id)
    {
        ItemName itemName;
        _itemNames.TryGetValue(id, out itemName);
        return itemName;
    }
}
