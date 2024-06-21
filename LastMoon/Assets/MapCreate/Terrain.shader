Shader "Custom/Terrain_URP"
{
    Properties
    {
        _TestTexture("Texture", 2D) = "white" {}
        _TestScale("Scale", Float) = 1
        [HideInInspector] _LayerCount("Layer Count", Int) = 8
        [HideInInspector] _MinHeight("Min Height", Float) = 0
        [HideInInspector] _MaxHeight("Max Height", Float) = 1

        [HideInInspector] _BaseColours("Base Colours", Color) = (1,1,1,1)
        [HideInInspector] _BaseStartHeights("Base Start Heights", Float) = 0
        [HideInInspector] _BaseBlends("Base Blends", Float) = 1
        [HideInInspector] _BaseColourStrength("Base Colour Strength", Float) = 1
        [HideInInspector] _BaseTextureScales("Base Texture Scales", Float) = 1
        [HideInInspector] _BaseTextures("Base Textures", 2DArray) = "" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            const static int maxLayerCount = 8;
            const static float epsilon = 1E-4;

            int _LayerCount;
            float3 _BaseColours[maxLayerCount];
            float _BaseStartHeights[maxLayerCount];
            float _BaseBlends[maxLayerCount];
            float _BaseColourStrength[maxLayerCount];
            float _BaseTextureScales[maxLayerCount];
            float _MinHeight;
            float _MaxHeight;

            TEXTURE2D(_TestTexture);
            SAMPLER(sampler_TestTexture);
            float _TestScale;

            TEXTURE2D_ARRAY(_BaseTextures);
            SAMPLER(sampler_BaseTextures);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldPos = worldPos;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            float inverseLerp(float a, float b, float value) {
                return saturate((value - a) / (b - a));
            }

            float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
                float3 scaledWorldPos = worldPos / _TestScale;

                float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, sampler_BaseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
                float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, sampler_BaseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
                float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(_BaseTextures, sampler_BaseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
                return xProjection + yProjection + zProjection;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float heightPercent = inverseLerp(_MinHeight, _MaxHeight, IN.worldPos.y);
                float3 blendAxes = abs(IN.worldNormal);
                blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

                float3 finalColor = float3(0, 0, 0);

                for (int i = 0; i < _LayerCount; i++) {
                    float drawStrength = inverseLerp(-_BaseBlends[i] / 2 - epsilon, _BaseBlends[i] / 2, heightPercent - _BaseStartHeights[i]);

                    float3 baseColour = _BaseColours[i] * _BaseColourStrength[i];
                    float3 textureColour = triplanar(IN.worldPos, _BaseTextureScales[i], blendAxes, i) * (1 - _BaseColourStrength[i]);

                    finalColor = lerp(finalColor, baseColour + textureColour, drawStrength);
                }

                half4 color = half4(finalColor, 1.0);
                return color;
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}