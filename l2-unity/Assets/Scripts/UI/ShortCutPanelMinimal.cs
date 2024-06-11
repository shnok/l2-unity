using FMOD.Studio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using static UnityEngine.Rendering.DebugUI.Table;


public class ShortCutPanelMinimal : MonoBehaviour
{
    private VisualTreeAsset[] arrayTemplate = new VisualTreeAsset[5];
    public VisualElement[] arrayPanels = new VisualElement[5];
    public ShortCutChildrenModel[] arrayRowsPanels = new ShortCutChildrenModel[5];
    private int sizeCell = 11;
    private bool initPosition = false;

    public void Start()
    {
        CretaeDemoInfo();
        arrayTemplate = CreateObject(arrayTemplate);
    }

    void Update()
    {
        if (this.initPosition == false)
        {
            Vector2 shortCutPosition = ShortCutPanel.Instance.GetPositionRoot();
            if (shortCutPosition.x != 0 & shortCutPosition.y != 0)
            {
                InitPosition(shortCutPosition.x, shortCutPosition.y, arrayPanels);

                if (arrayPanels != null)
                {
                    ShortCutPanel.Instance.setDrugChildren(arrayPanels);
                }

                initPosition = true;
            }

        }

    }
    public void SetResetPosition()
    {
        this.initPosition = false;
    }

    public void SetHidePanels()
    {
        HideElements(true, arrayPanels);
    }

    private void CretaeDemoInfo()
    {
        for (int i = 0; i < arrayRowsPanels.Length; i++)
        {
            if(i == 0)
            {
                int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                string[] img = { "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0914", "", "", "", "", "", "", "", "", "", "" };
                arrayRowsPanels[i] = new ShortCutChildrenModel(row, img);
            }
            if(i == 1)
            {
                int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                string[] img = { "Data/UI/ShortCut/demo_skills/skill0009", "Data/UI/ShortCut/demo_skills/skill5760", "", "", "", "", "", "", "", "", "", "" };
                arrayRowsPanels[i] = new ShortCutChildrenModel(row, img);
            }

            if(i == 2) { 
                int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                string[] img = { "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915" };
                arrayRowsPanels[i] = new ShortCutChildrenModel(row, img);
            }

            if (i == 3)
            {
                int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                string[] img = { "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760", "Data/UI/ShortCut/demo_skills/skill5760" };
                arrayRowsPanels[i] = new ShortCutChildrenModel(row, img);
            }

            if (i == 4)
            {
                int[] row = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 };
                string[] img = { "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "Data/UI/ShortCut/demo_skills/skill0915", "", "", "", "", "", "", "", "", "" };
                arrayRowsPanels[i] = new ShortCutChildrenModel(row, img);
            }

        }
    }
    private VisualTreeAsset[] CreateObject(VisualTreeAsset[] arrayTemplate)
    {
        for (int i = 0; i < arrayTemplate.Length; i++)
        {
            if (arrayTemplate[i] == null)
            {
                arrayTemplate[i] = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/ShortCutPanelMinimal");
            }
        }
        return arrayTemplate;
    }

    private static ShortCutPanelMinimal _instance;
    public static ShortCutPanelMinimal Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddWindow(VisualElement root)
    {
        if (arrayTemplate == null | arrayTemplate.Length == 0)
        {
            return;
        }


        arrayPanels = CreatePanels(arrayTemplate, arrayPanels);
        //setRows(arrayTemplate);
        HideElements(true, arrayPanels);
        //InitPosition(x_root, y_root, arrayPanels);
        AddPanelsToRoot(root, arrayPanels);
    }

    private void AddPanelsToRoot(VisualElement root, VisualElement[] arrayPanels)
    {
        for (int i = 0; i < arrayPanels.Length; i++)
        {
            root.Add(arrayPanels[i]);
        }

    }
    private VisualElement[] CreatePanels(VisualTreeAsset[] arrayTemplate, VisualElement[] arrayPanels)
    {
        //6 panels
        for (int panel = 0; panel < arrayTemplate.Length; panel++)
        {
            var shortCutMinimal = arrayTemplate[panel].Instantiate()[0];
            var minimal_panel = shortCutMinimal.Q(className: "minimal-panel");
            //12 cells
            // panel number panels | b number cell
            for (int cell =0; cell <= sizeCell; cell++)
            {
                var row = shortCutMinimal.Q(className: "row" + cell);
                var border = GetBorderRow(shortCutMinimal, cell);
                SetRows(border, row, panel, cell);
            }

            arrayPanels[panel] = minimal_panel;
        }

        return arrayPanels;
    }

    private void SetRows(VisualElement border  , VisualElement row, int i , int id_path)
    {
       SetImage(border , row, i, id_path);
    }

    private void VisibleBorder(VisualElement border, bool show)
    {
        if(border != null) border.visible = show;
    }

    private VisualElement GetBorderRow(VisualElement shortCutMinimal , int index)
    {
        return shortCutMinimal.Q(className: "border_row" + index);
    }
    private void SetImage(VisualElement border , VisualElement row , int i , int id_path)
    {
        string path = arrayRowsPanels[i].GetRowImgPath(id_path);
        Texture2D imgSource1 = Resources.Load<Texture2D>(path);
        if(imgSource1 != null)
        {
            VisibleBorder(border, true);
            row.style.backgroundImage = new StyleBackground(imgSource1);
        }
    }

    private void InitPosition(float x_root, float y_root, VisualElement[] arrayPanels)
    {
        for (int i = 0; i < arrayPanels.Length; i++)
        {
            //44 ширина short cut
            //25 отступ от верхней границы экрана
            arrayPanels[i].transform.position = new Vector2(x_root - 22, y_root);
        }

    }
    public void NewPosition(Vector2 vector, int activePanels , Vector2 originalRootVector )
    {
        VisualElement active = GetActivePanel(activePanels);


        if (active != null)
        {
            VisualElement activePanel = active;
            Vector2 newVector = GetVector(activePanels, vector);

            ResetDiff(activePanel);
            SyncRootPosition(activePanel, originalRootVector);

            HideElement(false, arrayPanels, activePanels);
            AddAnim(newVector, active);
        }
    }

    public void SyncRootPosition(VisualElement activePanel , Vector2 originalRootVector)
    {
        activePanel.transform.position = originalRootVector;
    }
    private void ResetDiff(VisualElement activePanel)
    {
        activePanel.style.left = 0;
        activePanel.style.top = 0;
    }

    // 0 - panels sync to shortcutpanel
    // else - panels sync to last shortcutminpanels
    private Vector2 GetVector(int activePanels , Vector2 vector)
    {
        if(activePanels == 0)
        {
            return new Vector2(vector.x - 30, vector.y);
        }
        else
        {
            return new Vector2(vector.x - 22, vector.y);
        }

    }

    public VisualElement GetLastElement(int index)
    {
        if (index >= 0 && index < arrayPanels.Length)
        {
            return arrayPanels[index];
        }
        return null;
    }

    public int GetLastPosition(int index)
    {
       return  arrayPanels.Length - 2;
    }

  

    public int Count()
    {
        return arrayPanels.Length;
    }

    private VisualElement GetActivePanel(int indexPanel)
    {
 
        if (indexPanel >= 0 && indexPanel < arrayPanels.Length)
        {
            return arrayPanels[indexPanel];
        }
        else
        {
            return arrayPanels[arrayPanels.Length - 1];
        }
      
    }
    public void AddAnim( Vector2 target_postion , VisualElement activeElement)
    {
        StartCoroutine(WaitAndStart(target_postion , activeElement));
    }
    private void HideElements(bool is_hide , VisualElement[] arrayPanels)
    {
        for(int i = 0; i < arrayPanels.Length; i++)
        {
            if (is_hide)
            {
                arrayPanels[i].style.display = DisplayStyle.None;
            }
            else
            {
                arrayPanels[i].style.display = DisplayStyle.Flex;
            }
        }

    }

    private void HideElement(bool is_hide, VisualElement[] arrayPanels , int index)
    {
        if(index >= arrayPanels.Length)
        {
            index = arrayPanels.Length - 1;
        }

        if(arrayPanels[index] != null)
        {
            if (is_hide)
            {
                arrayPanels[index].style.display = DisplayStyle.None;
            }
            else
            {
                arrayPanels[index].style.display = DisplayStyle.Flex;
            }
        }
          
    }

    private IEnumerator WaitAndStart( Vector2 target_postion , VisualElement activeElement)
    {
        while (true)
        {
            Vector2 source = activeElement.transform.position;
            Vector2 tempVector = Vector2.MoveTowards(source, target_postion, Time.deltaTime * 500);
            if (source.Equals(target_postion))
            {
                break;
            }

            activeElement.transform.position = tempVector;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public ShortCutChildrenModel[] GetArrayRowsPanels()
    {
        return arrayRowsPanels;
    }
    private void OnDestroy()
    {
        _instance = null;
    }

}
