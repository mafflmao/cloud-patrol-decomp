Shader "RenderFX/Skybox Blended" {
Properties {
 _Color ("Color", Color) = (0.5,0.5,0.5,1)
 _Blend ("Blend", Range(0,1)) = 0.5
 _Texture1 ("Texture 1", 2D) = "white" {}
 _Texture2 ("Texture 2", 2D) = "black" {}
}
SubShader { 
 Tags { "QUEUE"="Background" }
 Pass {
  Tags { "QUEUE"="Background" }
  Cull Off
  Fog { Mode Off }
  SetTexture [_Texture1] { combine texture }
  SetTexture [_Texture2] { ConstantColor (0,0,0,[_Blend]) combine texture lerp(constant) previous }
  SetTexture [_] { ConstantColor [_Color] combine previous * constant, constant alpha }
 }
}
Fallback "Diffuse"
}