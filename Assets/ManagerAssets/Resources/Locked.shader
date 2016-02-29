Shader "Hidden/Locked Effect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#pragma only_renderers d3d9 opengl gles

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _RampTex;
uniform half _RampOffset;

fixed4 frag (v2f_img i) : COLOR
{
	float blurAmount = 1.0/512.0;
	fixed4 original = fixed4(0.0);
	float dist01 = 1.0 * blurAmount;
				
	original += tex2D(_MainTex, float2(i.uv.x - dist01, i.uv.y - dist01));
	original += tex2D(_MainTex, float2(i.uv.x, i.uv.y - dist01));
	original += tex2D(_MainTex, float2(i.uv.x + dist01, i.uv.y - dist01));
	
	original += tex2D(_MainTex, float2(i.uv.x - dist01, i.uv.y));
	original += tex2D(_MainTex, i.uv);	
	original += tex2D(_MainTex, float2(i.uv.x + dist01, i.uv.y));
		
	original += tex2D(_MainTex, float2(i.uv.x - dist01, i.uv.y + dist01));
	original += tex2D(_MainTex, float2(i.uv.x, i.uv.y + dist01));
	original += tex2D(_MainTex, float2(i.uv.x + dist01, i.uv.y + dist01));
			
	original *= 1.0/9.0;

	fixed grayscale = Luminance(original.rgb);
	half2 remap = half2 (grayscale + _RampOffset, .5);
	fixed4 output = tex2D(_RampTex, remap);
	output.a = original.a;
	return output;
}
ENDCG

	}
}

Fallback "Unlit/Texture"

}