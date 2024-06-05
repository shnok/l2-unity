using FMOD.Studio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;


public class ShortCutPanelMinimal : MonoBehaviour
{
    private VisualTreeAsset[] arrayTemplate = new VisualTreeAsset[2];
    public VisualElement[] arrayPanels = new VisualElement[2];
    private bool initPosition = false;

    public void Start()
    {
        arrayTemplate = createObject(arrayTemplate);
    }

    void Update()
    {
        if (this.initPosition == false)
        {
            Vector2 shortCutPosition = ShortCutPanel.Instance.GetPositionRoot();
            if (shortCutPosition.x != 0 & shortCutPosition.y != 0)
            {
                InitPosition(shortCutPosition.x, shortCutPosition.y, arrayPanels);
                if(arrayPanels != null)
                {
                    ShortCutPanel.Instance.setDrugChildren(arrayPanels);
                }
                
                initPosition = true;
            }

        }

    }

    private VisualTreeAsset[] createObject(VisualTreeAsset[] arrayTemplate)
    {
        for (int i = 0; i < 2; i++)
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
        for (int i = 0; i < arrayTemplate.Length; i++)
        {
            var shortCutMinimal = arrayTemplate[i].Instantiate()[0];
            var minimal_panel = shortCutMinimal.Q(className: "minimal-panel");
            arrayPanels[i] = minimal_panel;
        }

        return arrayPanels;
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
    public void newPosition(Vector2 vector, int activePanels)
    {
        if (getActivePanel(activePanels) != null)
        {
            //каждую итерацию вычисляет шаг для движения
            //Vector2 test = Vector2.MoveTowards(result.transform.position, vector, Time.deltaTime * 10);
            // minimal_panel.transform.position = vector;
            Vector2 newVector = GetVector(activePanels, vector);
            HideElement(false, arrayPanels, activePanels);
            AddAnim(newVector, getActivePanel(activePanels));
        }
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

    public VisualElement getLastElement(int index)
    {
        if (index >= 0 && index < arrayPanels.Length)
        {
            return arrayPanels[index];
        }
        return null;
    }

    public int Count()
    {
        return arrayPanels.Length;
    }

    private VisualElement getActivePanel(int indexPanel)
    {
        if (indexPanel >= 0 && indexPanel < arrayPanels.Length)
        {
            return arrayPanels[indexPanel];
        }
        return null;
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
    private void OnDestroy()
    {
        _instance = null;
    }

}
