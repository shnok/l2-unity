using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRows
{
    private ModelItemDemo[] textures;
    public ModelRows(int rows , int[] gArr)
    {
        this.rows = rows;
        this.gArr = gArr;
        this.textures = new ModelItemDemo[gArr.Length];
    }

    public int lastActiveGRow { get; set; }
    public int rows { get; set; }
    public int[] gArr { get; set; }

    public void AddgArr(int imgbox_id,int addId)
    {
        gArr[imgbox_id] = addId;
    }

    public void AddInfo(int imgbox_id, Texture2D addTexture , int id_item , string nameItem)
    {

        textures[imgbox_id] = new ModelItemDemo(id_item, addTexture, nameItem); 
    }

    public ModelItemDemo GetInfo(int imgbox_id)
    {
        return textures[imgbox_id];
    }

 
}


