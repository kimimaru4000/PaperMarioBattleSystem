#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler s0;

Texture2D chargeTex;
sampler chargeSampler = sampler_state { Texture = <chargeTex>; };

float4 ChargeScroll(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	
	float4 chargeColor = tex2D(chargeSampler, coords);

	if (color.a)
		return chargeColor;

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL ChargeScroll();
	}
};