Shader "Custom/water"
{
    Properties
    {
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Cube("Cube", Cube) = ""{}

        //fresnel
        _FresnelPower("Fresnel Power", Range(0,10)) = 3
        _FresnelIntensity("Fresnel Intensity", Range(0,5)) = 1

        //Specular
        _SpColor("Specular Color", Color) = (1,1,1,1)
        _SpPower("Specular Power", Range(10,500)) = 150
        _SpIntensity("Specular Intensity", Range(0,10)) = 3

        //vertex Wave
        _WaveSpeed ("Wave Speed", Range(0.1, 2)) = 0.5
        _WaveFrequency ("Wave Frequency", Range(1, 10)) = 2
        _WaveAmplitude ("Wave Amplitude", Range(0.1, 2)) = 0.5

        [Space]
        _Alpha("Alpha", Range(0,1))=0.8
        _Tiling("Normal Tiling", Range(1,10)) = 1
        _Strength("Normal Strength", Range(0,2)) = 1

    }
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue"="Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert noshadow

        sampler2D _BumpMap;
        samplerCUBE _Cube;

        float _WaveSpeed;
        float _WaveFrequency;
        float _WaveAmplitude;

        struct Input
        {
            float2 uv_BumpMap;
            float3 worldRefl;
            float3 viewDir;
            INTERNAL_DATA
        };

        float _Alpha, _Penetration, _PenetrationThreshold;
        float _Tiling;
        float _Strength;
        float _CubeIntensity, _CubeBrightness;
        float _FresnelPower, _FresnelIntensity;
        float4 _SpColor;
        float _SpPower, _SpIntensity;

        float4 LightingWaterSpecular(SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
        {
            float3 H = normalize(lightDir + viewDir); // Binn Phong
            float spec = saturate(dot(H, s.Normal));
            spec = pow(spec, _SpPower);
    
            float4 col;
            col.rgb = spec * _SpColor.rgb * _SpIntensity * _LightColor0;
            col.a = s.Alpha + spec;
    
            return col;
        }

        void surf (Input IN, inout SurfaceOutput o) {
            float3 originNormal = o.Normal;
            float3 reflColor = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal));
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap * _Tiling)) * _Strength;

            float ndv = saturate(dot(normalize(o.Normal), IN.viewDir));
            float fresnel = 1. - pow(ndv, _FresnelPower) * _FresnelIntensity;

            o.Emission = reflColor * _CubeIntensity * fresnel + _CubeBrightness;

            float penet = pow(saturate(dot(originNormal, IN.viewDir)),
                            _PenetrationThreshold) * _Penetration;
            o.Alpha = _Alpha - penet;
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float time = _Time * _WaveSpeed;
            float waveValueA = sin(time + v.vertex.x * _WaveFrequency) * _WaveAmplitude;
            v.vertex.xyz = float3(v.vertex.x, v.vertex.y + waveValueA, v.vertex.z);
            v.normal = normalize(float3(v.normal.x + waveValueA, v.normal.y, v.normal.z));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
