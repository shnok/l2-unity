
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System;
using System.Collections;
using System.Diagnostics;


namespace Assets.Scripts.UI
{
    public class IconOverlay : MonoBehaviour
    {
        private VisualTreeAsset _overlayTemplate;
        public VisualElement result;

        private static IconOverlay _instance;
        public static IconOverlay Instance
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

        private void OnDestroy()
        {
            _instance = null;
        }

        void Start()
        {
            if (_overlayTemplate == null)
            {
                _overlayTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Inventory/IconOverlay");
            }
            if (_overlayTemplate == null)
            {
                UnityEngine.Debug.LogError("Could not load status window template.");
            }
        }

        public void AddWindow(VisualElement root)
        {
            if (_overlayTemplate == null)
            {
                return;
            }

            var statusWindowEle = _overlayTemplate.Instantiate()[0];

            result = statusWindowEle.Q(className: "image-opacity");
            // Debug.LogError(result.ToString());
            

            root.Add(statusWindowEle);
        }

        public void newPosition(Vector2 vector)
        {
            //var statusWindowEle = _overlayTemplate.Instantiate()[0];
            // VisualElement overlayImage = statusWindowEle.Q(className: "image-opacity");

            // var element = (VisualElement)evt.currentTarget;
            // float x = element.transform.position.x;
            // Debug.Log("");
            result.transform.position = vector;
            // Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);
            // overlayImage.style.left = _startMousePosition.x - diff.x;
            // overlayImage.style.top = _startMousePosition.y - diff.y;


            //evt.StopPropagation();
        }

        public void AddAnim(string[] img, float waitTime)
        {
            StartCoroutine(WaitAndStart(img , waitTime));
        }

        private IEnumerator WaitAndStart(string[] img , float waitTime)
        {
            for (int i = 0; i < img.Length; i++)
            {
                Texture2D imgSource1 = Resources.Load<Texture2D>(img[i]);
                setBackground(imgSource1);


                yield return new WaitForSeconds(waitTime);
            }

            newPosition(new Vector2(0, 0));
        }

        private void setBackground(Texture2D imgSource1)
        {
            if (imgSource1 != null)
            {
                result.style.backgroundImage = new StyleBackground(imgSource1);
            }
        }

    }
}
