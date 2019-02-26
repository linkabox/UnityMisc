Shader "CocoIsland/ActorShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _AlphaTex("Mask", 2D) = "white" {}
		[PerRendererData] _Additive("Additive", Color) = (0,0,0,1)
		[MaterialToggle] Hsv("Hsv Toggle", Float) = 1
		[PerRendererData] _Hue("Hue", Range(0,1)) = 0
		[PerRendererData] _Saturation("Saturation", Range(-1,1)) = 0
		[PerRendererData] _Value("Value", Range(-1,1)) = 0
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVertHSV
			#pragma fragment SpriteFragHSV
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ HSV_ON
			#include "UnitySprites.cginc"
#if HSV_ON
			#include "HueShift.cginc"
			fixed _Hue;
			fixed _Saturation;
			fixed _Value;
#endif
			fixed4 _Additive;

			v2f SpriteVertHSV(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

			#ifdef UNITY_INSTANCING_ENABLED
				IN.vertex.xy *= _Flip;
			#endif

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _RendererColor;

			#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
			#endif

				return OUT;
			}

			fixed4 SpriteFragHSV(v2f IN) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, IN.texcoord);
				fixed4 mask = tex2D(_AlphaTex, IN.texcoord);
				fixed alpha = min(color.a,mask.r);

				half3 outColor = color.rgb;
				#if HSV_ON
				outColor = shiftHSV(outColor, half3(_Hue,_Saturation,_Value));
				outColor = lerp(color.rgb,outColor,mask.g);
				#endif

				outColor +=  _Additive * outColor * 2;
				outColor *= alpha * IN.color;
				return fixed4(outColor,alpha);
			}
		ENDCG
		}
	}
}
