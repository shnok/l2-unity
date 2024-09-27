using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogongrpTable
{
    private static LogongrpTable _instance;
    public static LogongrpTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LogongrpTable();
            }

            return _instance;
        }
    }

    private List<Logongrp> _logongrps;

    public List<Logongrp> Logongrps { get { return _logongrps; } }

    public void Initialize()
    {
        Readgrps();
    }

    private void Readgrps()
    {
        _logongrps = new List<Logongrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Logongrp.txt");
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
                Logongrp logongrp = new Logongrp();

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
                        case "x":
                            logongrp.X = int.Parse(value);
                            break;
                        case "y":
                            logongrp.Y = int.Parse(value);
                            break;
                        case "z":
                            logongrp.Z = int.Parse(value);
                            break;
                        case "yaw":
                            logongrp.Yaw = int.Parse(value);
                            break;

                    }

                }

                logongrp.X = NumberUtils.FromIntToFLoat((int)logongrp.X);
                logongrp.Y = NumberUtils.FromIntToFLoat((int)logongrp.Y);
                logongrp.Z = NumberUtils.FromIntToFLoat((int)logongrp.Z);
                logongrp.Yaw = NumberUtils.FromIntToFLoat((int)logongrp.Yaw);

                _logongrps.Add(logongrp);
            }

            Debug.Log($"Successfully imported {_logongrps.Count} logongrp(s)");
        }
    }
}