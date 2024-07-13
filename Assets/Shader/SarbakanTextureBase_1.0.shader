Shader "Sarbakan/1.0/Texture/Base" {
Properties {
 _MainTex ("Main Texture", 2D) = "white" {}
 _MainColor ("Main Color x2", Color) = (0.5,0.5,0.5,0.5)
}
SubShader { 
 Tags { "QUEUE"="Geometry" "IGNOREPROJECTOR"="True" }
 Pass {
  Tags { "QUEUE"="Geometry" "IGNOREPROJECTOR"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  Cull Off
  SetTexture [_MainTex] { ConstantColor [_MainColor] combine texture * constant double }
  SetTexture [_] { combine previous * primary }
 }
}
}