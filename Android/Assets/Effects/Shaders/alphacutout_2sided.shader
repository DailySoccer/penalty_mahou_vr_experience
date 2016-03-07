Shader "Transparent/Cutout2sided" {

    

Properties

{

    _Color ("Main Color", Color) = (1,1,1,1)

    _SpecColor ("Spec Color", Color) = (1,1,1,0)

    _Emission ("Emmisive Color", Color) = (0,0,0,0)

    _Shininess ("Shininess", Range (0.1, 1)) = 0.7

    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

}

 

SubShader

{

    Tags {"Queue" = "Geometry+100"}

 

    Pass

    {

        Alphatest Greater [_Cutoff]

        Cull off

        

        Lighting On

        Material

        {

            Diffuse [_Color]

            Ambient [_Color]

            Shininess [_Shininess]

            Specular [_SpecColor]

            Emission [_Emission]    

        }

        

        SetTexture[_MainTex] {Combine texture * primary DOUBLE, texture} 

    }

}

 

}