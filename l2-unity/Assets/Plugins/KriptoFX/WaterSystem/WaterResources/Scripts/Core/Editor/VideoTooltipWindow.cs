#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace KWS
{
    public class VideoTooltipWindow : EditorWindow
    {
        public string VideoClipFileURI;

        GameObject tempGO;
        VideoClip clip;
        VideoPlayer player;
        Texture currentRT;

        public static void ShowWindow()
        {
            GetWindow<VideoTooltipWindow>();


        }

        void OnGUI()
        {
            Repaint();

            if (clip == null)
            {
                if (player == null)
                {
                    tempGO = new GameObject("WaterVideoWindowHelp");
                    tempGO.hideFlags = HideFlags.DontSave;

                    player = tempGO.AddComponent<VideoPlayer>();
                    player.playOnAwake = false;
                    player.url = VideoClipFileURI;
                    //player.clip = clip;
                    player.isLooping = true;

                    player.Prepare();

                    player.sendFrameReadyEvents = true;
                    player.frameReady += Player_frameReady;
                    player.Play();
                }

            }

            if (currentRT != null) EditorGUI.DrawPreviewTexture(new Rect(0, 0, position.width, position.height), currentRT);
        }

        void Update()
        {

        }

        private void Player_frameReady(VideoPlayer source, long frameIdx)
        {
            currentRT = source.texture;
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
            if (tempGO != null) KW_Extensions.SafeDestroy(tempGO);
        }
    }

}

#endif