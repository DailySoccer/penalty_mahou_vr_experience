Shader "Marmoset/Nature/Tree Creator Bark" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_GlossMap ("Gloss (A)", 2D) = "black" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_SpecInt ("Specular Intensity", Float) = 1.0
	_Fresnel ("Fresnel Falloff", Range(0.0,1.0)) = 0.0
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "RenderType"="TreeBark" }
	LOD 200
		
CGPROGRAM
#pragma surface surf BlinnPhong vertex:TreeVertBark addshadow nolightmap
#pragma exclude_renderers flash 
#pragma glsl
#pragma glsl_no_auto_normalization

#pragma multi_compile MARMO_GAMMA MARMO_LINEAR
#pragma target 3.0
#include "Tree.cginc"
#include "../../MarmosetCore.cginc"

#define MARMO_SKY_ROTATION
#define MARMO_NORMALMAP
//#define MARMO_SPECULAR_DIRECT

half4 		ExposureIBL;
samplerCUBE _DiffCubeIBL;
samplerCUBE _SpecCubeIBL;
#ifdef MARMO_SKY_ROTATION
float4x4	SkyMatrix;
#endif



sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _GlossMap;
half _Shininess;
half _SpecInt;
half _Fresnel;

struct Input {
	float2 uv_MainTex;
	float3 worldNormal;
	#ifdef MARMO_SPECULAR_DIRECT
		float3 viewDir;
		float3 worldRefl;
	#endif
	fixed4 color : COLOR;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * _Color.rgb * IN.color.a;
	o.Alpha = c.a;
	
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	float3 N = WorldNormalVector(IN, o.Normal);
	#ifdef MARMO_SKY_ROTATION
		N = mulVec3(SkyMatrix,N);
	#endif
	
	#ifdef MARMO_SPECULAR_DIRECT
		o.Gloss = tex2D(_GlossMap, IN.uv_MainTex).a;
		o.Specular = _Shininess;
		o.Gloss *= specEnergyScalar(o.Specular*128); 
		float3 E = IN.viewDir; //E is in whatever space N is
		E = normalize(E);
		half fresnel = splineFresnel(o.Normal, E, _SpecInt, _Fresnel);
		_SpecInt *= fresnel;
		o.Gloss *= _SpecInt;
		
		#ifdef MARMO_NORMALMAP
			float3 R = WorldReflectionVector(IN, o.Normal);
		#else 
			float3 R = IN.worldRefl;
		#endif
		#ifdef MARMO_SKY_ROTATION
			R = mulVec3(SkyMatrix,R);
		#endif
		float lod = glossLOD(o.Gloss, _Shininess * 6.0 + 2.0);
		o.Emission += glossCubeLookup(_SpecCubeIBL, R, lod) * ExposureIBL.y * _SpecInt;
	#endif
	
	o.Albedo *= ExposureIBL.w;
	o.Emission += diffCubeLookup(_DiffCubeIBL, N) * o.Albedo * ExposureIBL.x;
}
ENDCG
}

Dependency "OptimizedShader" = "Hidden/Marmoset/Nature/Tree Creator Bark Optimized"
FallBack "Diffuse"
}
