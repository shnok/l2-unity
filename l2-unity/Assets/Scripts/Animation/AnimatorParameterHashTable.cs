using System;
using UnityEngine;

public class AnimatorParameterHashTable
{
    private static AnimatorParameterHashTable _instance;
    public static AnimatorParameterHashTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnimatorParameterHashTable();
            }

            return _instance;
        }
    }

    public AnimatorParameterHashTable()
    {
        Initialize();
    }

    public int[] HumanoidHashTable { get; private set; }

    public void Initialize()
    {
        string[] humanoidParameters = Enum.GetNames(typeof(HumanoidAnimationEvent));
        HumanoidHashTable = new int[humanoidParameters.Length];

        for (int i = 0; i < humanoidParameters.Length; i++)
        {
            HumanoidHashTable[i] = Animator.StringToHash(humanoidParameters[i]);
        }
    }

    public int GetHumanoidParameterHash(int paramIndex)
    {
        return HumanoidHashTable[paramIndex];
    }
}