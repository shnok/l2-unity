using UnityEngine;

public class NpcName {
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private string _title;
    [SerializeField] private string _titleColor;

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Title { get => _title; set => _title = value; }
    public string TitleColor { get => _titleColor; set => _titleColor = value; }
}
