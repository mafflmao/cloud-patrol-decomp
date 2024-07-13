Shader "Mobile/Diffuse-Masked" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _MaskTex ("Mask (A)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant, texture alpha }
  SetTexture [_MaskTex] { ConstantColor [_Color] combine previous, texture alpha * constant alpha }
 }
}
Fallback "Diffuse"
}