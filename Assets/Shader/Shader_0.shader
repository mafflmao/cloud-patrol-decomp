Shader "Locked Skylander Portrait" {
Properties {
 _Color ("Color", Color) = (0,0.172549,0.321569,1)
 _TransformationWeights ("Transform Weights", Color) = (0.389,0.1465,0.4645,0.6)
 _MainTex ("Texture", 2D) = "" {}
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