Shader "Mobile/reflective-lit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Shininess ("Shininess", Range(0,1)) = 0.8
}
SubShader { 
 Pass {
  Lighting On
  Material {
   Ambient [_Color]
   Diffuse [_Color]
   Shininess [_Shininess]
  }
  SetTexture [_MainTex] { combine texture * primary double }
 }
 Pass {
  Lighting On
  Material {
   Ambient [_Color]
   Specular (1,1,1,1)
   Shininess [_Shininess]
  }
  Blend One One
  SetTexture [_MainTex] { combine texture * primary quad }
 }
}
}