#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

sampler s0;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 PoisonTint(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(s0, input.TextureCoordinates);
	
	//R and B are multiplied by some scale; G is untouched
	color.r = color.r * 0.6277;
	color.b = color.b * 0.3896;

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PoisonTint();
	}
};