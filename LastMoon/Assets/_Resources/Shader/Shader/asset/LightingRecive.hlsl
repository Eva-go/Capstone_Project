void LightingRecive_float(float3 WorldPos, out float3 Direction, out float3 Color, out float ShadowAtten) {
	#ifdef SHADERGRAPH_PREVIEW
		Direction = float3(1, 1, 1);
		Color = float3(1, 1, 1);
		ShadowAtten = 1.0f;
	#else
		Light light = GetMainLight();
		Direction = light.direction;
		Color = light.color;

		//ShadowAtten = light.shadowAttenuation;
		//Shadows.hlsl MainLightRealtimeShadow();
		ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
		half4 shadowParams = GetMainLightShadowParams();
		ShadowAtten = SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_LinearClampCompare), 
			TransformWorldToShadowCoord(WorldPos), shadowSamplingData, shadowParams, false);
	#endif
}
