// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

#ifndef MARMOSET_GRASS_CGINC
#define MARMOSET_GRASS_CGINC

sampler2D 	_MainTex;
fixed 		_Cutoff;
half4 		ExposureIBL;
#ifdef MARMO_DIFFUSE_IBL
samplerCUBE _DiffCubeIBL;
#endif
#ifdef MARMO_SKY_ROTATION
float4x4	SkyMatrix;
#endif

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 worldNormal;
};

void MarmosetGrassSurf(Input IN, inout SurfaceOutput o) {
	fixed4 diff = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	#ifdef MARMO_LINEAR
		//HACK: The terrain system is buggy and does not sRGB sample grass textures. We're on it though!
		diff.rgb = toLinearApprox3(diff.rgb);
	#endif
	diff.rgb *= ExposureIBL.w; //camera exposure is built into OUT.Albedo
	o.Albedo = diff.rgb;
	o.Alpha = diff.a;
	#ifdef MARMO_ALPHA_CLIP
		clip (o.Alpha - _Cutoff);
	#endif
	o.Alpha *= IN.color.a;
	
	#ifdef MARMO_DIFFUSE_IBL
		float3 N = IN.worldNormal;
		#ifdef MARMO_SKY_ROTATION
			N = mulVec3(SkyMatrix,N); //per-fragment matrix multiply, expensive
		#endif	
		half3 diffIBL = diffCubeLookup(_DiffCubeIBL, N);
		o.Emission = diffIBL * diff.rgb * ExposureIBL.x;
	#endif
}

#endif
