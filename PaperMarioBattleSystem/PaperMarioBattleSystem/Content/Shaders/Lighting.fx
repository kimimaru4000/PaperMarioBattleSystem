#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

sampler s0;

//The Light texture
Texture2D lightTex;

sampler lightSampler = sampler_state { Texture = <lightTex>; };

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 Lighting(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(s0, input.TextureCoordinates);
	float4 lightColor = tex2D(lightSampler, input.TextureCoordinates);

	return color * lightColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Lighting();
	}
};