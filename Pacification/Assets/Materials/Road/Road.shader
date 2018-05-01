Shader "Custom/Road" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular", Color) = (0.2, 0.2, 0.2)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "Queue" = "Geometry+1"
        }
        LOD 200
        Offset -1, -1

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows decal:blend vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #pragma multi_compile _ HEX_MAP_EDITOR

        #include "../Terrain/HexCellData.cginc"

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float2 visibility;
        };

        half _Glossiness;
        fixed3 _Specular;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input data) {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            float4 cell0 = GetCellData(v, 0);
            float4 cell1 = GetCellData(v, 1);

            data.visibility.x = cell0.x * v.color.x + cell1.x * v.color.y;
            data.visibility.x = lerp(0.25, 1, data.visibility.x);
            data.visibility.y = cell0.y * v.color.x + cell1.y * v.color.y;
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = _Color * IN.visibility.x;
            float blend = IN.uv_MainTex.x;
            blend = smoothstep(0.4, 0.7, blend);

            float explored = IN.visibility.y;
            o.Albedo = c.rgb;
            o.Specular = _Specular * explored;
            o.Smoothness = _Glossiness;
            o.Occlusion = explored;
            o.Alpha = blend * explored;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
