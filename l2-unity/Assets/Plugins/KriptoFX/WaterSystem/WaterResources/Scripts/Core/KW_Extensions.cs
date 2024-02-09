using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;

namespace KWS
{
    public static class KW_Extensions
    {

        static float prevRealTime;
        static float lastDeltaTime;
        static int renderdFrames;

        public enum AsyncInitializingStatusEnum
        {
            NonInitialized,
            StartedInitialize,
            Initialized,
            Failed,
            BakingStarted
        }

        public static Vector3 GetRelativeToCameraAreaPos(Camera cam, float areaSize, float waterLevel)
        {
            var pos = cam.transform.position;
            var bottomCornerViewWorldPos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0, cam.nearClipPlane));
            var cornerDir = (bottomCornerViewWorldPos - pos).normalized;
            var cornerDotX = Vector3.Dot(cornerDir, Vector3.right) / 2.0f + 0.5f;
            var cornerDotZ = Vector3.Dot(cornerDir, Vector3.forward) / 2.0f + 0.5f;

            float offsetX = Mathf.Lerp(-areaSize, areaSize, cornerDotX) * 0.5f;
            float offsetZ = Mathf.Lerp(-areaSize, areaSize, cornerDotZ) * 0.5f;
            return new Vector3(pos.x + offsetX, waterLevel, pos.z + offsetZ);
        }


        public static void SafeDestroy(params UnityEngine.Object[] components)
        {
            if (!Application.isPlaying)
            {
                foreach (var component in components)
                {
                    if (component != null) UnityEngine.Object.DestroyImmediate(component);
                }

            }
            else
            {
                foreach (var component in components)
                {
                    if (component != null) UnityEngine.Object.Destroy(component);
                }
            }
        }

        public static float TotalTime()
        {
            return (Application.isPlaying ? UnityEngine.Time.time : UnityEngine.Time.realtimeSinceStartup);
        }

        public static float DeltaTime()
        {
            if (Application.isPlaying)
            {
                return UnityEngine.Time.deltaTime;
            }
            else
            {
                return Mathf.Max(0.0001f, lastDeltaTime);
            }
        }

        private static bool _isEditorTimeUpdated;
        public static void UpdateEditorDeltaTime()
        {
            if (Application.isPlaying) return;

            if (_isEditorTimeUpdated) return;

            _isEditorTimeUpdated = true;
            lastDeltaTime = UnityEngine.Time.realtimeSinceStartup - prevRealTime;
            prevRealTime = UnityEngine.Time.realtimeSinceStartup;
        }

        public static void SetEditorDeltaTime()
        {
            _isEditorTimeUpdated = false;
        }

        public static string GetPathToStreamingAssetsFolder()
        {

            var streamingAssetData = Path.Combine(Application.streamingAssetsPath, "WaterSystemData");
            if (Directory.Exists(streamingAssetData)) return streamingAssetData;

            var dirs = Directory.GetDirectories(Application.dataPath, "WaterSystemData", SearchOption.AllDirectories);
            if (dirs.Length != 0)
            {
                if (Directory.Exists(dirs[0])) return dirs[0];
            }

            Debug.LogError("Can't find 'Assets/StreamingAssets/WaterSystemData' data folder");
            return string.Empty;
        }
        public static void SerializeToFile(string pathToFileWithoutExtenstion, object data)
        {
            var directory = Path.GetDirectoryName(pathToFileWithoutExtenstion);
            if (directory == null || data == null)
            {
                Debug.LogError("Can't find directory: " + pathToFileWithoutExtenstion);
                return;
            }
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(pathToFileWithoutExtenstion + ".txt", json);
        }

        public static async Task<T> DeserializeFromFile<T>(string pathToFileWithoutExtenstion)
        {
            try
            {
                var fileNameWithExtension = pathToFileWithoutExtenstion + ".txt";
                var directory = Path.GetDirectoryName(pathToFileWithoutExtenstion);
                if (!Directory.Exists(directory) || !File.Exists(fileNameWithExtension)) return default;

                using (var reader = File.OpenText(fileNameWithExtension))
                {
                    var json = await reader.ReadToEndAsync();
                    var data = JsonUtility.FromJson<T>(json);
                    return data;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error Deserialize From File " + Path.GetFileName(pathToFileWithoutExtenstion) +
                          Environment.NewLine + e.Message);
                return default;
            }
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
        public static async Task<Texture2D> ReadTextureFromFileAsync(string pathToFileWithoutExtension, bool linear = true, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
        {
            Texture2D tex;

            if (!File.Exists(pathToFileWithoutExtension + ".gz"))
            {
                Debug.LogError("Can't find the file: " + pathToFileWithoutExtension);
                return null;
            }
            try
            {
                //Debug.Log("read " + pathToFileWithoutExtension);
                using (var fileStream = File.Open(pathToFileWithoutExtension + ".gz", FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
                {
                    using (GZipStream gzip = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            await gzip.CopyToAsync(stream);

                            var rawTextureDataWithInfo = new byte[stream.Length];
                            stream.Seek(0, SeekOrigin.Begin);
                            await stream.ReadAsync(rawTextureDataWithInfo, 0, rawTextureDataWithInfo.Length);
                            {
                                var format = (TextureFormat)BitConverter.ToInt32(rawTextureDataWithInfo, 0);
                                int width = BitConverter.ToInt32(rawTextureDataWithInfo, 4);
                                int height = BitConverter.ToInt32(rawTextureDataWithInfo, 8);

                                var rawTextureData = new byte[rawTextureDataWithInfo.Length - 12];
                                Array.Copy(rawTextureDataWithInfo, 12, rawTextureData, 0, rawTextureData.Length);

                                //var gFormat = GraphicsFormatUtility.GetGraphicsFormat(format, false);
                                tex = new Texture2D(width, height, format, false, true);

                                tex.filterMode = filterMode;
                                tex.wrapMode = wrapMode;
                                tex.LoadRawTextureData(rawTextureData);
                                tex.Apply();
                            }
                            stream.Close();
                            gzip.Close();
                        }
                    }
                    fileStream.Close();
                }
                return tex;
            }
            catch (Exception e)
            {
                Debug.LogError("ReadTextureFromFileAsync error: " + e.Message);
                return null;
            }
        }

        public static void SaveTextureToFile(this Texture2D tex, string pathToFileWithoutExtension)
        {
            var directory = Path.GetDirectoryName(pathToFileWithoutExtension);
            if (directory == null)
            {
                Debug.LogError("Can't find directory: " + pathToFileWithoutExtension);
                return;
            }
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var fullPath = pathToFileWithoutExtension + ".gz";
            if (File.Exists(fullPath)) File.Delete(fullPath);
            try
            {
                //Debug.Log("save " + pathToFileWithoutExtension);
                using (var fileToCompress = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Delete))
                {
                    int w = tex.width;
                    int h = tex.height;
                    var rawTextureData = tex.GetRawTextureData();
                    var textureInfo = new List<byte>();

                    textureInfo.AddRange(BitConverter.GetBytes((uint)tex.format));
                    textureInfo.AddRange(BitConverter.GetBytes(w));
                    textureInfo.AddRange(BitConverter.GetBytes(h));
                    rawTextureData = Combine(textureInfo.ToArray(), rawTextureData);

                    using (GZipStream compressionStream = new GZipStream(fileToCompress, CompressionMode.Compress))
                    {
                        compressionStream.Write(rawTextureData, 0, rawTextureData.Length);
                        compressionStream.Close();
                    }
                    fileToCompress.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SaveTextureToFile error: " + e.Message);
                return;
            }
        }

        public static void SaveRenderTextureToFile(this RenderTexture rt, string pathToFileWithoutExtension, TextureFormat targetFormat)
        {
#if UNITY_EDITOR
            if (rt == null) return;
            var currentRT = RenderTexture.active;
            RenderTexture.active = rt;

            var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            UnityEditor.EditorUtility.CompressTexture(tex, targetFormat, UnityEditor.TextureCompressionQuality.Best);
            tex.SaveTextureToFile(pathToFileWithoutExtension);
            RenderTexture.active = currentRT;
            SafeDestroy(tex);
            //SafeDestroy(tex2);
#endif
        }

        [Flags]
        public enum WaterLogMessageType
        {
            Info = 1,
            Release = 2,
            Initialize = 3,
            InitializeRT = 4,
            Error = 5,
        }

        // private static WaterLogMessageType _debugFlags = WaterLogMessageType.Info | WaterLogMessageType.Initialize | WaterLogMessageType.InitializeRT | WaterLogMessageType.Error | WaterLogMessageType.Release;
        private static WaterLogMessageType _debugFlags = WaterLogMessageType.Info;

        public static void WaterLog(this object sender, string message, WaterLogMessageType logType = WaterLogMessageType.Info)
        {
#if KWS_DEBUG
            switch (logType)
            {
                case WaterLogMessageType.Info:
                    if (_debugFlags.HasFlag(WaterLogMessageType.Info))
                        Debug.Log($"<color=#0d224f>{sender} </color> : <color=#3e4044>{message}</color>");
                    break;
                case WaterLogMessageType.Release:
                    if (_debugFlags.HasFlag(WaterLogMessageType.Release))
                        Debug.Log($"<color=#0d224f>{sender} </color> : <color=#be651b>{message}</color>");
                    break;
                case WaterLogMessageType.Initialize:
                    if (_debugFlags.HasFlag(WaterLogMessageType.Initialize))
                        Debug.Log($"<color=#0d224f>{sender} </color> : <color=#1d6804>{message}</color>");
                    break;
                case WaterLogMessageType.InitializeRT:
                    if (_debugFlags.HasFlag(WaterLogMessageType.InitializeRT))
                        Debug.Log($"<color=#0d224f>{sender} </color> : <color=#1d6804>{message}</color>");
                    break;
                case WaterLogMessageType.Error:
                    if (_debugFlags.HasFlag(WaterLogMessageType.Error))
                        Debug.Log($"<color=#0d224f>{sender} </color> : <color=#ff0000>{message}</color>");
                    break;
                default:
                    Debug.Log($"<color=#0d224f>{sender} </color> : <color=#3e4044>{message}</color>");
                    break;
            }

#endif
        }

        public static void WaterLog(this object sender, params RenderTexture[] renderTextures)
        {
#if KWS_DEBUG
            foreach (var rt in renderTextures)
            {
                if (rt != null) WaterLog(sender, $"Initialized texture {rt.name} [{rt.width}] [{rt.height}] {rt.format} {rt.useMipMap}", WaterLogMessageType.InitializeRT);
            }
#endif
        }

        public static void WaterLog(this object sender, params KWS_RTHandle[] renderTextures)
        {
#if KWS_DEBUG
            foreach (var handle in renderTextures)
            {
                if (handle != null && handle.rt != null) WaterLog(sender, $"Initialized texture {handle.name} [{handle.rt.width}] [{handle.rt.height}] {handle.rt.format} {handle.rt.useMipMap}", WaterLogMessageType.InitializeRT);
            }
#endif
        }

    }
}