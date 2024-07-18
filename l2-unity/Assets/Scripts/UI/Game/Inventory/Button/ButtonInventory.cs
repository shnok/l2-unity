using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class ButtonInventory
{

    private InventoryWindow inventory;

    public ButtonInventory(InventoryWindow inventory)
    {
        this.inventory = inventory;
    }


    //public void RegisterButtonCloseWindow(VisualElement rootWindows, string buttonId)
    //{
    //    var btn = rootWindows.Q<Button>(className:buttonId);

    //    if (btn == null)
    //    {
    //        Debug.LogError(buttonId + " can't be found.");
    //        return;
    //    }

    //    btn.RegisterCallback<MouseUpEvent>(evt => {

    //        inventory.HideElements(true, rootWindows);
            

    //    }, TrickleDown.TrickleDown);
    //}

    //public void RegisterClickWindow(VisualElement contentId, VisualElement headerId)
    //{

    //    if (contentId == null | headerId == null)
    //    {
    //        Debug.LogError(contentId + " can't be found.");
    //        return;
    //    }

    //    contentId.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.BringFront();

    //    }, TrickleDown.TrickleDown);

    //    headerId.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.BringFront();
    //    }, TrickleDown.TrickleDown);
    //}


    //public void RegisterClickMenuAll(VisualElement tabElement)
    //{


    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(0);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}

    //public void RegisterClickMenuEquip(VisualElement tabElement)
    //{

    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(1);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}


    //public void RegisterClickMenuSupplies(VisualElement tabElement)
    //{

    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(2);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}


    //public void RegisterClickMenuCrafting(VisualElement tabElement)
    //{

    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(3);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}

    //public void RegisterClickMenuMisc(VisualElement tabElement)
    //{

    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(4);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}

    //public void RegisterClickMenuQuest(VisualElement tabElement)
    //{

    //    if (tabElement == null)
    //    {
    //        Debug.LogError(tabElement + " can't be found.");
    //        return;
    //    }

    //    tabElement.RegisterCallback<MouseDownEvent>(evt => {
    //        inventory.ChangeMenuSelect(5);
    //        AudioManager.Instance.PlayUISound("click_01");
    //    }, TrickleDown.TrickleDown);
    //}


    //public void RegisterClickInventoryCell(VisualElement grows)
    //{
    //    if (grows == null)
    //    {
    //        Debug.LogError(grows + " can't be found.");
    //        return;
    //    }

    //    grows.RegisterCallback<MouseDownEvent>(evt => {
    //        if(evt.currentTarget != null)
    //        {
    //            int id_mouse_button = evt.button;
    //            //leftClick - 0
    //            //rightCLick - 1
    //            if(id_mouse_button == 0)
    //            {
    //                var ve = (VisualElement)evt.currentTarget;
    //                inventory.SelectRows(ve);
    //            }else if (id_mouse_button == 1)
    //            {
    //                var ve = (VisualElement)evt.currentTarget;
    //                inventory.EquipItem(ve);
    //            }
              
    //        }
    //    }, TrickleDown.TrickleDown);

    //}


}
