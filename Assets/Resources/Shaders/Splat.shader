Shader "Custom/Splat" {
	Properties{

		_MainColor("Main Color", Color) = (1,1,1,0.5)
		_MainTex("Splat Map", 2D) = "white" {}
		[NoScaleOffset] _PrimTex("Primary Texture", 2D) = "white" { }
		[NoScaleOffset] _SecTex("Secondary Texture", 2D) = "gray" { }
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

			fixed4 _MainColor;
			sampler2D _MainTex;
			sampler2D _PrimTex;
			sampler2D _SecTex;
			float4 _MainTex_ST;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvSplat : TEXCOORD1;
			};


			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvSplat = v.texcoord;//Maintain the global position
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 splat = tex2D(_MainTex, i.uv);
				//fixed4 secondcol = tex2D(_SecondTex, i.uv);
				//return (texcol * _MainColor) + (secondcol * _SecondColor);
				//fixed4 base = texcol * _MainColor;
				//fixed4 overlay = secondcol * _SecondColor;
				//return base + overlay;

				return tex2D(_PrimTex, i.uv) * splat.r
					 + tex2D(_SecTex, i.uv) * (1-splat.r);
				//return splat;
			}
			ENDCG

			}
	}
}