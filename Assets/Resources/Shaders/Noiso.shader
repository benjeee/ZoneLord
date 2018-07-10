Shader "GPFlip"
{
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		GrabPass
		{
			"_BackgroundTexture"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 grabPos : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);

				float x = o.grabPos.x;
				float y = o.grabPos.y;
				float z = o.grabPos.z;

				o.grabPos.x = y;
				o.grabPos.y = x;

				return o;
			}

			sampler2D _BackgroundTexture;

			half4 frag(v2f i) : SV_Target
			{
				half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);

				//bgcolor.r = (1 + sin(_Time.y)) / 2;
				//bgcolor.g = (1 + cos(_Time.y)) / 2;
				//bgcolor.b = (bgcolor.r + bgcolor.b) / 2;
				
				//bgcolor.g = 1 - bgcolor.g;
				//bgcolor.b = 1 - bgcolor.b;
				

				return bgcolor;
			}
			ENDCG
		}
	}
}