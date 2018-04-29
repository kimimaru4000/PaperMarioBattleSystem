#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

sampler s0;

//The size of the texture
float2 textureSize;

//The time value to use for the shift
float shiftTime;

//The intensity of the shift
float intensity;

//The X amount to sample the pixels from
//The shift is scaled by this
float moveAmtX;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 SleepWave(VertexShaderOutput input) : COLOR0
{
	//Texel size
	float2 uvPix = float2(1 / textureSize.x, 1 / textureSize.y);

	//Shift the sampled X position a certain number of pixels based on the Y value and the intensity
	float finalShift = cos(shiftTime + (input.TextureCoordinates.y * intensity)) * moveAmtX;

	float4 color = tex2D(s0, input.TextureCoordinates + (float2(uvPix.x * finalShift, 0)));

	return color * input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL SleepWave();
	}
};