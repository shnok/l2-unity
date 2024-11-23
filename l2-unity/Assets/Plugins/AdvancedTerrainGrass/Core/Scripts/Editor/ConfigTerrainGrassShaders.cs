using UnityEngine;
using UnityEditor;

namespace AdvancedTerrainGrass {

    class ConfigTerrainGrassShaders
    {
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            Shader.SetGlobalVector("_AtgWindStrengthMultipliers", Vector4.zero);

            Shader.SetGlobalVector("_AtgGrassFadeProps", new Vector4(
                    80.0f * 80.0f,
                    1.0f / (20.0f * 20.0f * ((80.0f / 20.0f) * 2.0f)),
                    50.0f * 50.0f,
                    1.0f / (20.0f * 20.0f)
                ));

            Shader.SetGlobalVector("_AtgGrassShadowFadeProps", new Vector4(
                30.0f * 30.0f,
                1.0f / (15.0f * 15.0f),
                50.0f * 50.0f,
                1.0f / (20.0f * 20.0f)
            ));
        }
    }
}