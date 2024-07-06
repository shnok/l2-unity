using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelRows
{
    public ModelRows(int rows , int[] gArr)
    {
        this.rows = rows;
        this.gArr = gArr;
    }

    public int lastActiveGRow { get; set; }
    public int rows { get; set; }
    public int[] gArr { get; set; }
}
