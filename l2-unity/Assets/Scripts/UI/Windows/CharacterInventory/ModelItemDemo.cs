using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelItemDemo
{
    public ModelItemDemo(int item_id, Texture2D texture , string name)
    {
        this.item_id = item_id;
        this.texture = texture;
        this.name = name;
    }

    public int item_id { get; set; }
    public Texture2D texture { get; set; }
    public string name { get; set; }


}