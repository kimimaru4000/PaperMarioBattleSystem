#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;
float4 colorVal = (0, 0, 0, 0);

float4 SetColor(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	color.rgb = colorVal.rgb;

	return color;
}

float4 GrayScale(float2 coords: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(s0, coords);
		color.rgb = color.r;
    return color;
}

float4 ColorFlip(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	color.rgb = color.bgr;

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL GrayScale();
	}
	//pass P1
	//{
	//	PixelShader = compile PS_SHADERMODEL ColorFlip();
	//}
};