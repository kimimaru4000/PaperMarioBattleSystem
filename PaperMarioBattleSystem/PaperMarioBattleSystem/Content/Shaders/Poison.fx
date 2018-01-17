#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

float4 PoisonTint(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	
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