using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRows
{
    private ModelItemDemo[] modelItem;
    
    public ModelRows(int rows , int[] gArr)
    {
        this.rows = rows;
        this.gArr = gArr;
        this.modelItem = new ModelItemDemo[gArr.Length];
    }

    public int lastActiveGRow { get; set; }
    public int rows { get; set; }
    public int[] gArr { get; set; }

    public void AddgArr(int imgbox_id,int addId)
    {
        gArr[imgbox_id] = addId;
    }

    public void AddInfo(int imgbox_id, DemoL2JItem demol2j)
    {

        modelItem[imgbox_id] = new ModelItemDemo(demol2j); 
    }

    public ModelItemDemo GetInfo(int imgbox_id)
    {
        return modelItem[imgbox_id];
    }

 
}


