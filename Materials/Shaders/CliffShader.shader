Shader "Tile/Cliff Shader (Basic)" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
	
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		sampler2D _MainTex;
		uniform float4 _Color;
		
		
		struct appdata{
			float4 position : POSITION;
			float2 uv1 : TEXCOORD0;
			float2 uv2 : TEXCOORD1;
		};
		
		struct v2f{
			float4 position : POSITION;
			float2 uv1 : TEXCOORD0;
			float2 uv2 : TEXCOORD1;
		};
		
		v2f vert(appdata v){
			v2f o;
			
			o.position = mul(UNITY_MATRIX_MVP, v.position);
			
			o.uv1 = v.uv1.xy;
			o.uv2 = v.uv2.xy;
			
			return o;
		}
		
		float4 frag(v2f i) : COLOR{
			float4 c = tex2D(_MainTex, i.uv2) * _Color;
			return c;
		}
		
		
		ENDCG
		}
	}
	FallBack "Diffuse"
}
