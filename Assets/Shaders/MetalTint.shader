Shader "Custom/MetalTint" 
{
	Properties 
	{
	    
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _Color ("Color", Color) = (1,1,1,1) 
	}

	SubShader 
	{
	    Pass 
	    {

	        ColorMaterial AmbientAndDiffuse
	        Lighting On
	        
	        SetTexture [_MainTex] {  	
	            Combine texture * primary
	        }
	        SetTexture [_MainTex] {
	        	constantColor [_Color]
	        	Combine previous * constant
	        	//Combine previous * constant DOUBLE, previous * constant
	        }
	        //SetTexture [_OreTex] {
	        //	Combine texture * previous
	        	
	        //}
	    }
	}

	Fallback "Diffuse", 1
}