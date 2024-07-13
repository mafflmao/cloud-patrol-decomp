Shader "Hidden/Render Overdraw Z" {
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Color (0.1,0.04,0.02,0)
  Fog { Mode Off }
  Blend One One
 }
}
SubShader { 
 Tags { "RenderType"="Transparent" }
 Pass {
  Tags { "RenderType"="Transparent" }
  Color (0.1,0.04,0.02,0)
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend One One
 }
}
}