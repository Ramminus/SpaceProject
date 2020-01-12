Shader "Instanced/InstancedSurfaceShader" {
    Properties{
        _Colour("Colour", color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Index("Colour Index", Int) = 0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model
            #pragma surface surf Standard addshadow fullforwardshadows
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            float4 _Colours[16];
            float _Numbers[16];
            int number;
            
            struct Input {
                float2 uv_MainTex;
            };

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            uniform StructuredBuffer<float4> positionBuffer;
        #endif

            void rotate2D(inout float2 v, float r)
            {
                float s, c;
                sincos(r, s, c);
                v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
            }
            float4 GetColour(int index) {
                for (int i = 0; i < number; i++) {
                    if (index < _Numbers[i])return _Colours[i];
                }
                return float4(1.0f, 0.0f, 0.0f, 1.0f);
            }
            void setup()
            {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                float4 data = positionBuffer[unity_InstanceID];

                //float rotation = data.w * data.w * _Time.y * 0.5f;
                //rotate2D(data.xz, rotation);

                unity_ObjectToWorld._11_21_31_41 = float4(data.w, 0, 0, 0);
                unity_ObjectToWorld._12_22_32_42 = float4(0, data.w, 0, 0);
                unity_ObjectToWorld._13_23_33_43 = float4(0, 0, data.w, 0);
                unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
                unity_WorldToObject = unity_ObjectToWorld;
                unity_WorldToObject._14_24_34 *= -1;
                unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
            #endif
            }

            half _Glossiness;
            half _Metallic;
            float4 _Colour;
            
            int _Index;
            void surf(Input IN, inout SurfaceOutputStandard o) {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                _Colour = GetColour(unity_InstanceID);
#endif
                o.Albedo = _Colour;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = 1;
            }
            ENDCG
        }
            FallBack "Diffuse"
}