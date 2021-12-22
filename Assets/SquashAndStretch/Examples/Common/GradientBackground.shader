/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

Shader "Squash & Stretch Kit Example/GradientBackground"
{
  Properties
  {
    _TopColor    ("Top Color", Color)    = (0.7, 0.7, 0.7, 1.0)
    _BottomColor ("Bottom Color", Color) = (0.4, 0.4, 0.4, 1.0)
  }

  SubShader
  {
    Cull Off ZWrite Off ZTest Off

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      struct appdata
      {
        float4 vertex : POSITION;
      };

      struct v2f
      {
        float4 vertex : SV_POSITION;
        float  colorT : TEXCOORD0;
      };

      float4 _TopColor;
      float4 _BottomColor;

      v2f vert (appdata v)
      {
        v2f o;
        o.vertex = v.vertex;
        o.colorT = (o.vertex.y + 1.0f) / 2.0f;
        return o;
      }
      
      float4 frag (v2f i) : COLOR0
      {
        return lerp(_TopColor, _BottomColor, i.colorT);
      }
      ENDCG
    }
  }
}
