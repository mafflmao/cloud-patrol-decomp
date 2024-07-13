Shader "Transparent/Cutout/VertexLit" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecColor ("Spec Color", Color) = (1,1,1,0)
 _Emission ("Emissive Color", Color) = (0,0,0,0)
 _Shininess ("Shininess", Range(0.1,1)) = 0.7
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}