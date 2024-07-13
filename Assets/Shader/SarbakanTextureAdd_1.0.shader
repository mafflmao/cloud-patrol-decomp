Shader "Sarbakan/1.0/Texture/Add" {
Properties {
 _MainTex ("Main Texture", 2D) = "white" {}
 _MainColor ("Main Color x2", Color) = (0.5,0.5,0.5,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  SetTexture [_MainTex] { ConstantColor [_MainColor] combine texture * constant double }
  SetTexture [_] { combine previous * primary }
 }
}
}