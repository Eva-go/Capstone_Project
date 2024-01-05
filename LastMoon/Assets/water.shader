Shader "Custom/water"
{
    Properties
    {
        _Color ("Wave Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _WaveSpeed ("Wave Speed", Range(0.1, 2)) = 0.5
        _WaveFrequency ("Wave Frequency", Range(1, 10)) = 2
        _WaveAmplitude ("Wave Amplitude", Range(0.1, 2)) = 0.5
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert noshadow

        sampler2D _MainTex;
        fixed4 _Color;
        float _WaveSpeed;
        float _WaveFrequency;
        float _WaveAmplitude;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl;
            INTERNAL_DATA
        };

        void surf (Input IN, inout SurfaceOutput o) {
            // Calculate wave displacement
            float waveDisplacement = sin(_Time.y * _WaveSpeed + IN.uv_MainTex.x * _WaveFrequency) * _WaveAmplitude;

            // Apply wave to Y coordinate
            IN.uv_MainTex.y += waveDisplacement;

            // Sample texture
            float3 fWorldReflectionVector = WorldReflectionVector(IN, o.Normal).xyz;        
            o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, fWorldReflectionVector).rgb * unity_SpecCube0_HDR.r;        
        
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Output final color
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        void vert(inout appdata_full v)
        {
            v.vertex.y += sin((abs(v.texcoord.x * 2.0f - 1.0f)*10.0f) + _Time.y*0.8f)*0.12f +
            sin((abs(v.texcoord.y * 2.0f -1.0f)*10.0f)+ _Time.y*0.8f)*0.12f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
