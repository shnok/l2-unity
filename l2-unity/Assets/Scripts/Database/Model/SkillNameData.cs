using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class SkillNameData
{
    [SerializeField] public int _id;
    [SerializeField] public int _level;
    [SerializeField] public int _subLevel;
    [SerializeField] public string _name;
    [SerializeField] public string _desc;
    [SerializeField] public int _prev_skill_id;
    

    public int Id { get => _id; set => _id = value; }
    public int Level { get => _level; set => _level = value; }
    public int SubLevel { get => _subLevel; set => _subLevel = value; }
    public string Name { get => _name; set => _name = value; }
    public string Desc { get => _desc; set => _desc = value; }
    public int PrevSkillId { get => _prev_skill_id; set => _prev_skill_id = value; }

}
