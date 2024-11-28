Shader "Custom/SliderFillShader"
{
    Properties
    {
        _FillColor ("Fill Color", Color) = (0, 0.5, 1, 1)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 1)
        _BorderWidth ("Border Width", Float) = 2.0
        _MainTex ("Base Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform float _BorderWidth;
            uniform float4 _FillColor;
            uniform float4 _BorderColor;
            uniform sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // ��ȡ��ǰ���ص���ɫ
                half4 texColor = tex2D(_MainTex, i.uv);
                
                // �����������ӽ��䡢�����Ч��
                half4 fillColor = _FillColor;

                // ����ڱ�Ե����ʹ�ñ߿���ɫ
                if (i.uv.x < _BorderWidth || i.uv.x > (1.0 - _BorderWidth) ||
                    i.uv.y < _BorderWidth || i.uv.y > (1.0 - _BorderWidth))
                {
                    fillColor = _BorderColor;
                }

                return fillColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
