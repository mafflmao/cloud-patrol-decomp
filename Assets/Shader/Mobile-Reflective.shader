Shader "Mobile/reflective" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Reflect ("Reflection", 2D) = "black" { TexGen SphereMap }
}
SubShader { 
 Pass {
  Name "BASE"
  BindChannels {
   Bind "vertex", Vertex
   Bind "normal", Normal
   Bind "texcoord", TexCoord0
  }
  Color [_Color]
  SetTexture [_MainTex] { combine texture * previous }
  SetTexture [_Reflect] { combine texture + previous }
 }
}
}