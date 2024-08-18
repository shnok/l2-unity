using System;
using System.Collections.Generic;

public static class SkillTimeArray
{

    private static List<String> list;
    public static String[] GetArrayImage()
    {

        if(list == null)
        {
             list = new List<string>();

            for(int i=1; i < 358; i++)
            {
                string path = "Data/UI/ShortCut/skill_time/cooltime" + i++;
                list.Add(path);
            }

           
        }

 
        return list.ToArray();
    }
}
