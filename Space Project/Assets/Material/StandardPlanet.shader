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
		_LightTexture("Light Texture", 2D) = "white" {}
		[HDR]_LightColour("Light Colour", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
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
		uniform sampler2D _LightTexture;
		uniform float4 _LightTexture_ST;
		uniform float4 _LightColour;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_16_0_g5 = ( ase_worldPos * 100.0 );
			float3 crossY18_g5 = cross( ase_worldNormal , ddy( temp_output_16_0_g5 ) );
			float3 worldDerivativeX2_g5 = ddx( temp_output_16_0_g5 );
			float dotResult6_g5 = dot( crossY18_g5 , worldDerivativeX2_g5 );
			float crossYDotWorldDerivX34_g5 = abs( dotResult6_g5 );
			float2 uv_HeightMap = i.uv_texcoord * _HeightMap_ST.xy + _HeightMap_ST.zw;
			float temp_output_20_0_g5 = tex2D( _HeightMap, uv_HeightMap ).r;
			float3 crossX19_g5 = cross( ase_worldNormal , worldDerivativeX2_g5 );
			float3 break29_g5 = ( sign( crossYDotWorldDerivX34_g5 ) * ( ( ddx( temp_output_20_0_g5 ) * crossY18_g5 ) + ( ddy( temp_output_20_0_g5 ) * crossX19_g5 ) ) );
			float3 appendResult30_g5 = (float3(break29_g5.x , -break29_g5.y , break29_g5.z));
			float3 normalizeResult39_g5 = normalize( ( ( crossYDotWorldDerivX34_g5 * ase_worldNormal ) - appendResult30_g5 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g5 = mul( ase_worldToTangent, normalizeResult39_g5);
			float3 temp_output_3_40 = worldToTangentDir42_g5;
			float3 temp_output_16_0_g4 = ( ase_worldPos * 100.0 );
			float3 crossY18_g4 = cross( ase_worldNormal , ddy( temp_output_16_0_g4 ) );
			float3 worldDerivativeX2_g4 = ddx( temp_output_16_0_g4 );
			float dotResult6_g4 = dot( crossY18_g4 , worldDerivativeX2_g4 );
			float crossYDotWorldDerivX34_g4 = abs( dotResult6_g4 );
			float2 panner20 = ( 1.0 * _Time.y * _CloudPanSpeed + i.uv_texcoord);
			float2 Offset11 = ( ( _Height - 1 ) * float3( 0,0,0 ).xy * _CloudScale ) + panner20;
			float4 tex2DNode13 = tex2D( _CloudMap, Offset11 );
			float temp_output_20_0_g4 = tex2DNode13.r;
			float3 crossX19_g4 = cross( ase_worldNormal , worldDerivativeX2_g4 );
			float3 break29_g4 = ( sign( crossYDotWorldDerivX34_g4 ) * ( ( ddx( temp_output_20_0_g4 ) * crossY18_g4 ) + ( ddy( temp_output_20_0_g4 ) * crossX19_g4 ) ) );
			float3 appendResult30_g4 = (float3(break29_g4.x , -break29_g4.y , break29_g4.z));
			float3 normalizeResult39_g4 = normalize( ( ( crossYDotWorldDerivX34_g4 * ase_worldNormal ) - appendResult30_g4 ) );
			float3 worldToTangentDir42_g4 = mul( ase_worldToTangent, normalizeResult39_g4);
			float3 lerpResult33 = lerp( temp_output_3_40 , worldToTangentDir42_g4 , tex2DNode13.r);
			float3 lerpResult47 = lerp( temp_output_3_40 , lerpResult33 , (float)_HasAtmosphere);
			o.Normal = lerpResult47;
			float2 uv_AlbedoTexture = i.uv_texcoord * _AlbedoTexture_ST.xy + _AlbedoTexture_ST.zw;
			float4 tex2DNode2 = tex2D( _AlbedoTexture, uv_AlbedoTexture );
			float4 lerpResult23 = lerp( tex2DNode2 , tex2DNode13 , tex2DNode13.r);
			float4 lerpResult46 = lerp( tex2DNode2 , lerpResult23 , (float)_HasAtmosphere);
			o.Albedo = lerpResult46.rgb;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV35 = dot( normalize( ase_worldNormal ), ase_worldViewDir );
			float fresnelNode35 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV35, 5.0 ) );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult48 = dot( ase_worldlightDir , ase_worldNormal );
			float clampResult53 = clamp( dotResult48 , 0.0 , 1.0 );
			float2 uv_LightTexture = i.uv_texcoord * _LightTexture_ST.xy + _LightTexture_ST.zw;
			float4 tex2DNode55 = tex2D( _LightTexture, uv_LightTexture );
			float clampResult57 = clamp( -dotResult48 , 0.0 , 1.0 );
			float4 lerpResult61 = lerp( ( fresnelNode35 * _AtmosphereColour * clampResult53 ) , ( tex2DNode55 * _LightColour * clampResult57 ) , tex2DNode55.r);
			o.Emission = lerpResult61.rgb;
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
0;0;1920;1019;1831.578;1151.93;1.492566;True;False
Node;AmplifyShaderEditor.TexCoordVertexDataNode;26;-1392.326,-819.7923;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;22;-1394.971,-674.7682;Inherit;False;Property;_CloudPanSpeed;CloudPanSpeed;4;0;Create;True;0;0;False;0;0,0;0.02,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;20;-1121.971,-774.7682;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-564.8876,-551.9189;Inherit;False;Property;_CloudScale;CloudScale;3;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-559.9351,-627.1859;Inherit;False;Property;_Height;Height;5;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;37;-501.771,416.9822;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;36;-502.5101,557.5606;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;4;-640.445,-11.93896;Inherit;True;Property;_HeightMap;HeightMap;1;0;Create;True;0;0;False;0;None;5e0a897a24c87e240aa4c50719175048;False;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ParallaxMappingNode;11;-224.7109,-639.0206;Inherit;False;Normal;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;48;-145.367,345.6222;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;12;-696.8014,-1079.471;Inherit;True;Property;_CloudMap;Cloud Map;2;0;Create;True;0;0;False;0;None;48200e5d22739fb4a836b6013b135bb2;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.NegateNode;56;-36.82019,726.8579;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-502.1802,-255.3835;Inherit;True;Property;_AlbedoTexture;AlbedoTexture;0;0;Create;True;0;0;False;0;None;9b116def74d450549af70af4d532ec8f;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;13;37.09344,-683.8058;Inherit;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-371.5598,-11.85468;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;39;307.9246,114.6206;Inherit;False;Property;_AtmosphereColour;AtmosphereColour;6;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0.3658953,1.566032,2.79544,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;34;206.7868,-193.341;Inherit;False;Normal From Height;-1;;4;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;59;-251.8202,961.8579;Inherit;False;Property;_LightColour;Light Colour;9;1;[HDR];Create;True;0;0;False;0;0,0,0,0;5.216475,2.239534,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;55;-454.8202,735.8579;Inherit;True;Property;_LightTexture;Light Texture;8;0;Create;True;0;0;False;0;-1;None;207896b95f4759f4a8d58d9a87126409;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-205.445,-254.939;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;57;354.1798,697.8579;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;3;8.883689,18.42267;Inherit;False;Normal From Height;-1;;5;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;35;131.6989,447.6454;Inherit;False;Standard;WorldNormal;ViewDir;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;53;151.1798,315.8579;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;43;535.9402,-410.2835;Inherit;False;Property;_HasAtmosphere;HasAtmosphere;7;1;[Enum];Create;True;2;No;0;Yes;1;0;False;0;1;1;0;1;INT;0
Node;AmplifyShaderEditor.LerpOp;33;545.2844,-227;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;23;513.7338,-721.4879;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;571.1798,524.8579;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;671.6511,290.3635;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;47;810.67,-441.3222;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;61;938.1798,412.8579;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;46;811.2952,-710.4691;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1267.408,-641.8073;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;StandardPlanet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;26;0
WireConnection;20;2;22;0
WireConnection;11;0;20;0
WireConnection;11;1;32;0
WireConnection;11;2;15;0
WireConnection;48;0;37;0
WireConnection;48;1;36;0
WireConnection;56;0;48;0
WireConnection;13;0;12;0
WireConnection;13;1;11;0
WireConnection;10;0;4;0
WireConnection;34;20;13;1
WireConnection;2;0;1;0
WireConnection;57;0;56;0
WireConnection;3;20;10;1
WireConnection;35;0;36;0
WireConnection;53;0;48;0
WireConnection;33;0;3;40
WireConnection;33;1;34;40
WireConnection;33;2;13;1
WireConnection;23;0;2;0
WireConnection;23;1;13;0
WireConnection;23;2;13;1
WireConnection;58;0;55;0
WireConnection;58;1;59;0
WireConnection;58;2;57;0
WireConnection;38;0;35;0
WireConnection;38;1;39;0
WireConnection;38;2;53;0
WireConnection;47;0;3;40
WireConnection;47;1;33;0
WireConnection;47;2;43;0
WireConnection;61;0;38;0
WireConnection;61;1;58;0
WireConnection;61;2;55;1
WireConnection;46;0;2;0
WireConnection;46;1;23;0
WireConnection;46;2;43;0
WireConnection;0;0;46;0
WireConnection;0;1;47;0
WireConnection;0;2;61;0
ASEEND*/
//CHKSM=B0BC1853052279A09EC2B9E460DB043417B7601B