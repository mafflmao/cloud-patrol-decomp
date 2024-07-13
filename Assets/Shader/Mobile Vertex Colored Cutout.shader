Shader "Mobile/Vertex Colored Cutout" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,1)
 _Emission ("Emmisive Color", Color) = (0,0,0,0)
 _Shininess ("Shininess", Range(0.01,1)) = 0.7
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader { 
 Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="True" "RenderType"="TransparentCutout" }
 Pass {
  Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="True" "RenderType"="TransparentCutout" }
  Material {
   Emission [_Emission]
   Specular [_SpecColor]
   Shininess [_Shininess]
  }
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater [_Cutoff]
  ColorMaterial AmbientAndDiffuse
  SetTexture [_MainTex] { combine texture * primary, texture alpha * primary alpha }
  SetTexture [_MainTex] { ConstantColor [_Color] combine previous * constant double, previous alpha * constant alpha }
 }
}
Fallback " VertexLit"
}