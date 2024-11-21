//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace JBooth.MicroSplat
{
   public interface IRenderLoopAdapter 
   {
      string GetDisplayName();
      string GetRenderLoopKeyword();
      string GetShaderExtension();
      void Init(string[] paths);
      StringBuilder WriteShader(string[] features,
            MicroSplatShaderGUI.MicroSplatCompiler compiler,
            MicroSplatShaderGUI.MicroSplatCompiler.AuxShader auxShader,
            string name,
            string baseName);
      string GetVersion();
   }
}
