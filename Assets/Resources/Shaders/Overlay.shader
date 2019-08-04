Shader "Custom/DetailOverlay" {
	Properties{

		_MainTex("Texture", 2D) = "white" { }
		_DetailTex("Detail Texture", 2D) = "gray" {}
	}


		SubShader{

			Tags
			{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			}

			Pass {

			Blend SrcAlpha OneMinusSrcAlpha


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DetailTex;
			float4 _MainTex_ST;
			float4 _DetailTex_ST;

			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvDetail : TEXCOORD1;
				float4 color : COLOR;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvDetail = TRANSFORM_TEX(v.texcoord, _DetailTex);
				o.color = v.color;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 texcol = tex2D(_MainTex, i.uv) * i.color;
				//fixed4 secondcol = tex2D(_SecondTex, i.uv);
				//return (texcol * _MainColor) + (secondcol * _SecondColor);
				//fixed4 base = texcol * _MainColor;
				//fixed4 overlay = secondcol * _SecondColor;
				//return base + overlay;
				float sinTheta = 0.25f * sin(_Time.y);
				float cosThetaSlow = 0.25f * cos(_Time.y/2);
				float2 uvDetail = float2(i.uvDetail.x + sin(i.uvDetail.y * (6 + 2 * cosThetaSlow)) + _Time.x, i.uvDetail.y);
				return texcol * tex2D(_DetailTex, uvDetail) * (1 + 0.20*sinTheta);
			}
			ENDCG

			}
	}
}