using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonEquipInventory 
{
    public void RegisterClickEquipCell(VisualElement grows)
    {
        if (grows == null)
        {
            Debug.LogError(grows + " can't be found.");
            return;
        }

        grows.RegisterCallback<MouseDownEvent>(evt => {
            if (evt.currentTarget != null)
            {
                int id_mouse_button = evt.button;
                //leftClick - 0
                //rightCLick - 1
                if (id_mouse_button == 0)
                {
                    var ve = (VisualElement)evt.currentTarget;
                    Debug.Log("CLICK EQUIP INVT Left");
                }
                else if (id_mouse_button == 1)
                {
                    var ve = (VisualElement)evt.currentTarget;
                    Debug.Log("CLICK EQUIP INVT Right");
                }

            }
        }, TrickleDown.TrickleDown);

    }

}
