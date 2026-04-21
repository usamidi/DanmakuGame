Shader "Custom/InstancedColorShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _SoftnessMin ("Range Min", Range(0, 1)) = 0.7
        _SoftnessMax ("Range Max", Range(0, 1)) = 0.9
        _SmoothPower ("光滑指数", Range(0, 1.0)) = 0.8
        _TransparencyPower  ("Transparency Power", Range(0, 1.0)) = 0.01
        _Rotation ("Rotation (Degrees)", Range(0, 360)) = 0
        //_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        // 关键 2: 设置混合模式 (Blend SrcAlpha OneMinusSrcAlpha)
        // 这是最标准的透明度叠加算法
        Blend SrcAlpha OneMinusSrcAlpha
        
        // 关键 3: 关闭深度写入 (ZWrite Off)
        // 否则透明物体会遮挡它身后的东西
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // 关键：开启实例化
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // 关键：定义实例ID输入
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // 关键：定义实例ID输出
            };


            // 关键：定义实例化属性块
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SoftnessMin, _SoftnessMax;
            float _TransparencyPower;
            float _SmoothPower;
            float _Rotation;
            // 辅助函数：旋转 UV
            
            float2 RotateUV (float2 uv, float rotation) {
                // 将角度转为弧度
                float angle = radians(rotation);
                float s, c;
                sincos(angle, s, c);
                // 构造旋转矩阵
                float2x2 rotationMatrix = float2x2(c, -s, s, c);
                // 旋转需要以中心点 (0.5, 0.5) 为轴
                uv -= 0.5;
                uv = mul(uv, rotationMatrix);
                uv += 0.5;
                return uv;
            }

            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); // 关键：必须设置ID
                UNITY_TRANSFER_INSTANCE_ID(v, o); // 关键：传输ID
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(RotateUV(v.uv, _Rotation), _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i); // 关键：在片元着色器中也要设置ID

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                // 计算亮度 (0为黑，1为白)
                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                float width = fwidth(luminance);

                float alpha = smoothstep(0.3, 1.0, pow(luminance, _TransparencyPower)) * col.a;
                float edge = smoothstep(_SoftnessMin - width, _SoftnessMax + width, pow(luminance, _SmoothPower));

                fixed3 tintedRGB = lerp(color.rgb, col.rgb, edge);

                return fixed4(tintedRGB, alpha * color.a);
            }
            ENDCG
        }
    }
}
