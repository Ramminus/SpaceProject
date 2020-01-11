// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "StandardPlanet"
{
	Properties
	{
		_AlbedoTexture("AlbedoTexture", 2D) = "white" {}
		_HeightMap("HeightMap", 2D) = "bump" {}
		_CloudMap("Cloud Map", 2D) = "white" {}
		_CloudScale("CloudScale", Float) = 0
		_CloudPanSpeed("CloudPanSpeed", Vector) = (0,0,0,0)
		_Height("Height", Float) = 0
		[HDR]_AtmosphereColour("AtmosphereColour", Color) = (0,0,0,0)
		[Enum(No,0,Yes,1)]_HasAtmosphere("HasAtmosphere", Int) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
			float3 viewDir;
		};

		uniform sampler2D _HeightMap;
		uniform float4 _HeightMap_ST;
		uniform sampler2D _CloudMap;
		uniform float2 _CloudPanSpeed;
		uniform float _Height;
		uniform float _CloudScale;
		uniform int _HasAtmosphere;
		uniform sampler2D _AlbedoTexture;
		uniform float4 _AlbedoTexture_ST;
		uniform float4 _AtmosphereColour;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_16_0_g3 = ( ase_worldPos * 100.0 );
			float3 crossY18_g3 = cross( ase_worldNormal , ddy( temp_output_16_0_g3 ) );
			float3 worldDerivativeX2_g3 = ddx( temp_output_16_0_g3 );
			float dotResult6_g3 = dot( crossY18_g3 , worldDerivativeX2_g3 );
			float crossYDotWorldDerivX34_g3 = abs( dotResult6_g3 );
			float2 uv_HeightMap = i.uv_texcoord * _HeightMap_ST.xy + _HeightMap_ST.zw;
			float temp_output_20_0_g3 = tex2D( _HeightMap, uv_HeightMap ).r;
			float3 crossX19_g3 = cross( ase_worldNormal , worldDerivativeX2_g3 );
			float3 break29_g3 = ( sign( crossYDotWorldDerivX34_g3 ) * ( ( ddx( temp_output_20_0_g3 ) * crossY18_g3 ) + ( ddy( temp_output_20_0_g3 ) * crossX19_g3 ) ) );
			float3 appendResult30_g3 = (float3(break29_g3.x , -break29_g3.y , break29_g3.z));
			float3 normalizeResult39_g3 = normalize( ( ( crossYDotWorldDerivX34_g3 * ase_worldNormal ) - appendResult30_g3 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g3 = mul( ase_worldToTangent, normalizeResult39_g3);
			float3 temp_output_3_40 = worldToTangentDir42_g3;
			float3 temp_output_16_0_g2 = ( ase_worldPos * 100.0 );
			float3 crossY18_g2 = cross( ase_worldNormal , ddy( temp_output_16_0_g2 ) );
			float3 worldDerivativeX2_g2 = ddx( temp_output_16_0_g2 );
			float dotResult6_g2 = dot( crossY18_g2 , worldDerivativeX2_g2 );
			float crossYDotWorldDerivX34_g2 = abs( dotResult6_g2 );
			float2 panner20 = ( 1.0 * _Time.y * _CloudPanSpeed + i.uv_texcoord);
			float2 Offset11 = ( ( _Height - 1 ) * i.viewDir.xy * _CloudScale ) + panner20;
			float4 tex2DNode13 = tex2D( _CloudMap, Offset11 );
			float temp_output_20_0_g2 = tex2DNode13.r;
			float3 crossX19_g2 = cross( ase_worldNormal , worldDerivativeX2_g2 );
			float3 break29_g2 = ( sign( crossYDotWorldDerivX34_g2 ) * ( ( ddx( temp_output_20_0_g2 ) * crossY18_g2 ) + ( ddy( temp_output_20_0_g2 ) * crossX19_g2 ) ) );
			float3 appendResult30_g2 = (float3(break29_g2.x , -break29_g2.y , break29_g2.z));
			float3 normalizeResult39_g2 = normalize( ( ( crossYDotWorldDerivX34_g2 * ase_worldNormal ) - appendResult30_g2 ) );
			float3 worldToTangentDir42_g2 = mul( ase_worldToTangent, normalizeResult39_g2);
			float3 lerpResult33 = lerp( temp_output_3_40 , worldToTangentDir42_g2 , tex2DNode13.r);
			float3 lerpResult47 = lerp( temp_output_3_40 , lerpResult33 , (float)_HasAtmosphere);
			o.Normal = lerpResult47;
			float2 uv_AlbedoTexture = i.uv_texcoord * _AlbedoTexture_ST.xy + _AlbedoTexture_ST.zw;
			float4 tex2DNode2 = tex2D( _AlbedoTexture, uv_AlbedoTexture );
			float4 lerpResult23 = lerp( tex2DNode2 , tex2DNode13 , tex2DNode13.r);
			float4 lerpResult46 = lerp( tex2DNode2 , lerpResult23 , (float)_HasAtmosphere);
			o.Albedo = lerpResult46.rgb;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV35 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode35 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV35, 5.0 ) );
			o.Emission = ( fresnelNode35 * _AtmosphereColour ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
0;0;1920;1019;849.4299;1023.722;1.3;True;False
Node;AmplifyShaderEditor.TexCoordVertexDataNode;26;-1392.326,-819.7923;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;22;-1394.971,-674.7682;Inherit;False;Property;_CloudPanSpeed;CloudPanSpeed;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;20;-1121.971,-774.7682;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;30;-557.5954,-470.4296;Inherit;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;15;-564.8876,-551.9189;Inherit;False;Property;_CloudScale;CloudScale;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-559.9351,-627.1859;Inherit;False;Property;_Height;Height;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;12;-696.8014,-1079.471;Inherit;True;Property;_CloudMap;Cloud Map;2;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-640.445,-11.93896;Inherit;True;Property;_HeightMap;HeightMap;1;0;Create;True;0;0;False;0;None;f5a5135fd2ea45648aaca7d609f39789;False;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ParallaxMappingNode;11;-224.7109,-639.0206;Inherit;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;13;37.09344,-683.8058;Inherit;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;1;-502.1802,-255.3835;Inherit;True;Property;_AlbedoTexture;AlbedoTexture;0;0;Create;True;0;0;False;0;None;f5a5135fd2ea45648aaca7d609f39789;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;10;-371.5598,-11.85468;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-205.445,-254.939;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;3;8.883689,18.42267;Inherit;False;Normal From Height;-1;;3;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;36;-341.5101,292.5606;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;34;170.9651,-247.0735;Inherit;False;Normal From Height;-1;;2;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;35;162.6989,299.6454;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;39;390.1171,18.79712;Inherit;False;Property;_AtmosphereColour;AtmosphereColour;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;33;545.2844,-227;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;23;513.7338,-721.4879;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IntNode;43;534.4477,-481.9262;Inherit;False;Property;_HasAtmosphere;HasAtmosphere;7;1;[Enum];Create;True;2;No;0;Yes;1;0;False;0;1;0;0;1;INT;0
Node;AmplifyShaderEditor.LerpOp;46;756.0702,-659.722;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;687.9416,81.85446;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;37;148.5292,134.331;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;47;810.67,-441.3222;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1267.408,-641.8073;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;StandardPlanet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;26;0
WireConnection;20;2;22;0
WireConnection;11;0;20;0
WireConnection;11;1;32;0
WireConnection;11;2;15;0
WireConnection;11;3;30;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;10;0;4;0
WireConnection;2;0;1;0
WireConnection;3;20;10;1
WireConnection;34;20;13;1
WireConnection;35;0;36;0
WireConnection;33;0;3;40
WireConnection;33;1;34;40
WireConnection;33;2;13;1
WireConnection;23;0;2;0
WireConnection;23;1;13;0
WireConnection;23;2;13;1
WireConnection;46;0;2;0
WireConnection;46;1;23;0
WireConnection;46;2;43;0
WireConnection;38;0;35;0
WireConnection;38;1;39;0
WireConnection;47;0;3;40
WireConnection;47;1;33;0
WireConnection;47;2;43;0
WireConnection;0;0;46;0
WireConnection;0;1;47;0
WireConnection;0;2;38;0
ASEEND*/
//CHKSM=3D9D9C69DFBD27A31E3E543302A9773BF9CAB352