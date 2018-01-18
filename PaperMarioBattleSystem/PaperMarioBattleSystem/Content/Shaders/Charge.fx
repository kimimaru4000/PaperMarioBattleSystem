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

float chargeAlpha;
float4 entityColor;

float2 chargeOffset;
float diff;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 ChargeScroll(VertexShaderOutput input) : COLOR0
{
	//1. The Charge texture is applied for 1 second in total, including the alpha's fade in and fade out times
	//2. Then the Charge texture is not applied for 1 second
	//3. Repeat
	
	//The Charge texture timing is global; all characters charged, regardless if they just charged or not, will have the same
	//parts of the texture applied to them at the same time, with the same alpha value used for the texture

	//The Charge texture wraps two times in this period

	float4 color = tex2D(s0, input.TextureCoordinates);

	//Check the Y pixel
	//For each Y, go down the Y on the Charge texture
	//So Y = 0, then the pixel chosen is at coords.y = 0
	//Wrap around every 64, which is the height of the Charge texture (use variable later)
	//So, if the current pixel is at 65, we would choose pixel 1 on the Charge texture

	float2 coords = float2(input.TextureCoordinates.x, input.TextureCoordinates.y / diff);

	//Frac will wrap the values on the Charge texture from 0 to (less than) 1
	float4 chargeColor = tex2D(chargeSampler, frac(coords + chargeOffset));

	if (color.a)
	{
		float4 multChargeAlpha = mul(chargeColor, mul(chargeAlpha, entityColor.a));
		return multChargeAlpha + color;
	}

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL ChargeScroll();
	}
};