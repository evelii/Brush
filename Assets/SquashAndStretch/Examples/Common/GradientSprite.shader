/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

Shader "Squash & Stretch Kit Example/GradientSprite"
{
  Properties
  {
    _TopColor    ("Top Color", Color)    = (0.7, 0.7, 0.7, 1.0)
    _BottomColor ("Bottom Color", Color) = (0.4, 0.4, 0.4, 1.0)
    _Bounds      ("Bounds", Vector)      = (0.0, 1.0, 1.0, 1.0)
    _MainTex     ("", 2D)                = "white" {}
  }

  SubShader
  {
    Tags
    { 
      "Queue"="Transparent" 
      "IgnoreProjector"="True" 
      "RenderType"="Transparent" 
      "PreviewType"="Plane"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata_t
      {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
      };

      struct v2f
      {
        float4 vertex  : SV_POSITION;
        fixed4 color   : COLOR;
        half2 texcoord : TEXCOORD0;
      };

      float4 _TopColor;
      float4 _BottomColor;
      float4 _Bounds;

      v2f vert(appdata_t v)
      {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = v.texcoord;
        o.color = lerp(_BottomColor, _TopColor, clamp((mul(unity_ObjectToWorld, v.vertex).y - _Bounds.x) / (_Bounds.y - _Bounds.x), 0.0f, 1.0f));
        return o;
      }

      sampler2D _MainTex;

      fixed4 frag(v2f i) : SV_Target
      {
        return float4(i.color.rgb, tex2D(_MainTex, i.texcoord).a);
      }
      ENDCG
    }
  }
}
