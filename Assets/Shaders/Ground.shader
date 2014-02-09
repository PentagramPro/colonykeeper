Shader "Custom/Ground" 
{
	Properties 
	{
	    
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	   // _OreTex ("Ore (RGB)", 2D) = "white" {}
	    _Color ("Color", Color) = (1,1,1,1) 
	}

	SubShader 
	{
	    Pass 
	    {

	
			//BindChannels {
			  // Bind "Vertex", vertex
			  // Bind "texcoord", texcoord0
			 //  Bind "texcoord1", texcoord1
			   
			  // Bind "Color", Color 
			//} 
	        ColorMaterial AmbientAndDiffuse
	        Lighting Off
	        
	        SetTexture [_MainTex] {
	        	
	            Combine texture * primary
	            
	            
	        }
	        SetTexture [_MainTex] {
	        	constantColor [_Color]
	        	Combine previous lerp(constant) constant
	        }
	        //SetTexture [_OreTex] {
	        //	Combine texture * previous
	        	
	        //}
	    }
	}

	Fallback " VertexLit", 1
}