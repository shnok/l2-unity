Shader "Hidden/Kronnect/Beautify" {
Properties {
    _FlareTex("Flare Texture", 2D) = "white" {}
    _OverlayTex("Lens Dirt Texture", 2D) = "black" {}
    _Color("", Color) = (1,1,1)
    _BlueNoise("Blue Noise", 2D) = "black" {}
    _BokehData2("", Vector) = (1,1,1,1)
    _BokehData3("", Vector) = (1,1,1,1)
    _BlurMask("Blur Mask", 2D) = "white" {}
    _FlipY("Flip Y", Float) = 1
}

Subshader {	

    Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
    LOD 100
    ZWrite Off ZTest Always Blend Off Cull Off

    HLSLINCLUDE
    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    ENDHLSL

  Pass { // 0 
      Name "Raw Copy (Point Filtering)"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCopy
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 1 
      Name "Compare View"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCompare
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 2  
      Name "Main Beautify Pass (core)"
      HLSLPROGRAM
      #pragma vertex VertBeautify
      #pragma fragment FragBeautify
      #pragma multi_compile_local_fragment __ BEAUTIFY_ACES_TONEMAP BEAUTIFY_ACES_FITTED_TONEMAP BEAUTIFY_AGX_TONEMAP
      #pragma multi_compile_local_fragment __ BEAUTIFY_LUT BEAUTIFY_3DLUT BEAUTIFY_NIGHT_VISION BEAUTIFY_THERMAL_VISION
	  #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM
      #pragma multi_compile_local_fragment __ BEAUTIFY_DIRT
      #pragma multi_compile_local_fragment __ BEAUTIFY_DEPTH_OF_FIELD BEAUTIFY_DOF_TRANSPARENT BEAUTIFY_CABERRATION
      #pragma multi_compile_local_fragment __ BEAUTIFY_PURKINJE
      #pragma multi_compile_local_fragment __ BEAUTIFY_VIGNETTING BEAUTIFY_VIGNETTING_MASK
      #pragma multi_compile_local_fragment __ BEAUTIFY_EYE_ADAPTATION
      #pragma multi_compile_local_fragment __ BEAUTIFY_COLOR_TWEAKS
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #pragma multi_compile_local_fragment __ BEAUTIFY_DITHER
      #pragma multi_compile_local_fragment __ BEAUTIFY_SHARPEN
      #pragma multi_compile_local_fragment __ BEAUTIFY_FRAME BEAUTIFY_FRAME_MASK
      #pragma multi_compile_local_fragment __ BEAUTIFY_EDGE_AA BEAUTIFY_OUTLINE
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 3
      Name "Extract Luminance"
      HLSLPROGRAM
      #pragma vertex VertLum
      #pragma fragment FragLum
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
	  #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_USE_DEPTH
	  #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_USE_LAYER BEAUTIFY_BLOOM_USE_LAYER_INCLUSION
      #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_PROP_THRESHOLDING
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 4 
      Name "Debug bloom"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragDebugBloom
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 5 
      Name "Blur horizontally"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #define BEAUTIFY_BLUR_HORIZ
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    
      
  Pass { // 6 
      Name "Blur vertically"
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }    

  Pass { // 7 
      Name "Bloom compose"
	  HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBloomCompose
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 8 
      Name "Resample"
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResample
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 9 
      Name "Combine resample"
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResample
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }   

  Pass { // 10 
      Name "Bloom extract luminance with antiflicker"
	  HLSLPROGRAM
      #pragma vertex VertCrossLum
      #pragma fragment FragLumAntiflicker
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_USE_DEPTH
      #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_USE_LAYER BEAUTIFY_BLOOM_USE_LAYER_INCLUSION
      #pragma multi_compile_local_fragment __ BEAUTIFY_BLOOM_PROP_THRESHOLDING
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

   Pass { // 11 
      Name "Resample Anamorphic Flares"
	  HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragResampleAF
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }

  Pass { // 12 
      Name "Combine AF"
	  HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCombine
      #define COMBINE_BLOOM
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

  Pass { // 13 
      Name "Compute Screen Lum"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragScreenLum
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 14 
      Name "Reduce Screen Lum"
      HLSLPROGRAM
      #pragma vertex VertCross
      #pragma fragment FragReduceScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 15 
      Name "Blend Screen Lum"
      Blend SrcAlpha OneMinusSrcAlpha
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBlendScreenLum
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }      
  
  Pass { // 16 
      Name "Simple Blend"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBlend
      #include "BeautifyPPSEA.hlsl"
      ENDHLSL
  }  

  Pass { // 17 
      Name "AF Lum"
      HLSLPROGRAM
      #pragma vertex VertLum
      #pragma fragment FragLum
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
	  #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER_INCLUSION
      #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_PROP_THRESHOLDING
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }  

  Pass { // 18 
      Name "AF Lum AntiFlicker"
      HLSLPROGRAM
      #pragma vertex VertCrossLum
      #pragma fragment FragLumAntiflicker
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
	  #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER_INCLUSION
      #pragma multi_compile_local_fragment __ BEAUTIFY_ANAMORPHIC_PROP_THRESHOLDING
      #define USE_AF_THRESHOLD
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  } 

 Pass { // 19 
      Name "Sun Flares"
      HLSLPROGRAM
      #pragma vertex VertSF
      #pragma fragment FragSF
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_SF_USE_GHOSTS
      #pragma multi_compile_local_fragment __ BEAUTIFY_SF_OCCLUSION_SIMPLE BEAUTIFY_SF_OCCLUSION_SMOOTH
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }
  
 Pass { // 20 
      Name "Sun Flares Additive"
      HLSLPROGRAM
      #pragma vertex VertSF
      #pragma fragment FragSFAdditive
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_SF_USE_GHOSTS
      #pragma multi_compile_local_fragment __ BEAUTIFY_SF_OCCLUSION_SIMPLE BEAUTIFY_SF_OCCLUSION_SMOOTH
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }

 Pass { // 21 
      Name "DoF CoC"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_DOF_TRANSPARENT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 22 
      Name "DoF CoC Debug"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCoCDebug
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_DOF_TRANSPARENT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 
 
  Pass { // 23 
      Name "DoF Blur"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

  Pass { // 24 
      Name "DoF Blur wo/Bokeh"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBlurNoBokeh
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

  Pass { // 25
      Name "DoF Blur Horizontally"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #define BEAUTIFY_BLUR_HORIZ
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

  Pass { // 26
      Name "DoF Blur Vertically"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlurCoC
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }    

  Pass { // 27 
      Name "Raw Copy (Bilinear Filtering)"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCopy
      #define USE_BILINEAR
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 28 
      Name "Bloom Exclusion Layer Debug"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragDebugBloomExclusionLayer
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }
 
  Pass { // 29 
      Name "DoF Debug Transparent"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragDoFDebugTransparent
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_DOF_TRANSPARENT
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  } 

  Pass { // 30 
      Name "Chromatic Aberration Custom Pass"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment fragChromaticAberration
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment __ BEAUTIFY_TURBO
      #define BEAUTIFY_CABERRATION 1
      #include "BeautifyCAberration.hlsl"
      ENDHLSL
  } 

  Pass { // 31 
      Name "Outline Detection Pass"
      HLSLPROGRAM
      #pragma vertex VertOutline
      #pragma fragment fragOutline
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma multi_compile_local_fragment _ BEAUTIFY_DEPTH_FADE
      #pragma multi_compile_local_fragment _ BEAUTIFY_OUTLINE_CUSTOM_DEPTH BEAUTIFY_OUTLINE_OBJECT_ID
      #pragma multi_compile_local_fragment _ BEAUTIFY_OUTLINE_MIN_SEPARATION
      #include "BeautifyPPSOutline.hlsl"
      ENDHLSL
  }

  Pass { // 32 
      Name "Outline Blur Horizontally (depth aware)"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #define BEAUTIFY_BLUR_HORIZ
      #include "BeautifyPPSOutline.hlsl"
      ENDHLSL
  }

  Pass { // 33  
      Name "Outline Blur Vertically (depth aware)"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSOutline.hlsl"
      ENDHLSL
  }

  Pass { // 34 
      Name "Outline Blend Pass"
	  Blend SrcAlpha OneMinusSrcAlpha
      HLSLPROGRAM
      #pragma vertex VertOutline
      #pragma fragment FragCopy
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyPPSOutline.hlsl"
      ENDHLSL
  }

  Pass { // 35 
      Name "Mask Blur"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCopyWithMask
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }    


  Pass { // 36 
      Name "DoF Threshold for bokeh"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragThreshold
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }

  Pass { // 37
      Name "DoF Additive"
      Blend One One
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCopyBokeh
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }

  Pass { // 38 
      Name "DoF Blur bokeh"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragBlurSeparateBokeh
      #include "BeautifyPPSDoF.hlsl"
      ENDHLSL
  }

  Pass { // 39
      Name "Anamorphic Flares Exclusion Layer Debug"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragDebugAnamorphicFlaresExclusionLayer
      #include "BeautifyPPSLum.hlsl"
      ENDHLSL
  }
  
  Pass { // 40 
      Name "Copy with MiniView"
      HLSLPROGRAM
      #pragma vertex VertOS
      #pragma fragment FragCopyWithMiniView
      #include "BeautifyCore.hlsl"
      ENDHLSL
  }

  Pass { // 41
      Name "Sun Flares Occlusion Test"
      Blend SrcAlpha OneMinusSrcAlpha
      HLSLPROGRAM
      #pragma vertex VertSF
      #pragma fragment FragSFOcclusion
      #pragma multi_compile_local_fragment _ BEAUTIFY_SF_OCCLUSION_INIT
      #include "BeautifyPPSSF.hlsl"
      ENDHLSL
  }
}
FallBack Off
}
