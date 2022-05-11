Shader "Holistic/Hologram"{
    Properties{
        _RimColour ("Rim Colour", Color) = (0,0.5,0.5,0.0)
        _RimPower("Rim Power", Range(0.5,8.0)) = 1.0
    }
    
    SubShader{
        Tags {"Queue" = "Transparent"}
        
        Pass{
            ZWrite On
            ColorMask 0
        }
        
        
        //Pass 2
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        struct Input{
            float3 viewDir;
            float3 worldPos;
        };
        
        float4 _RimColour;
        float _RimPower;
        
        void surf (Input IN, inout SurfaceOutput o){
            half rim = 1- saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColour.rgb * pow(rim,_RimPower);
            o.Alpha = pow(rim, _RimPower);
        
        }
        ENDCG
  }
    
    FallBack "Diffuse"
}