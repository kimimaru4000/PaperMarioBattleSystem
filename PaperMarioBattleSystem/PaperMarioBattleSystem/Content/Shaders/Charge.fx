#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler s0;

//The Charge texture
Texture2D chargeTex;
sampler chargeSampler = sampler_state { Texture = <chargeTex>; };

//The alpha value to make the Charge texture
float chargeAlpha;
//The color of the object that's being rendered
float4 objColor;

//The offset to sample the Charge texture from
float2 chargeOffset;
//The ratio between the Charge texture and the object's full texture (Ex. spritesheet)
//The lower this value is, the closer together the colors of the Charge texture will be
float chargeTexRatio;
//The texture coordinates of the frame the object is rendering in the spritesheet
float2 objFrameOffset;

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

	//Offset the rendered object's current frame from the top of the spritesheet so the charge effect is always consistent
	float2 scaledCoords = input.TextureCoordinates - objFrameOffset;

	//Check the Y pixel
	//For each Y, go down the Y on the Charge texture
	//So Y = 0, then the pixel chosen is at coords.y = 0
	//Wrap around every 64, which is the height of the Charge texture (use variable later)
	//So, if the current pixel is at 65, we would choose pixel 1 on the Charge texture
	float2 coords = float2(scaledCoords.x, scaledCoords.y / chargeTexRatio);

	//Frac will wrap the values on the Charge texture from 0 to (less than) 1
	float4 chargeColor = tex2D(chargeSampler, frac(coords + chargeOffset));

	if (color.a)
	{
		float4 multChargeAlpha = chargeColor * (chargeAlpha * objColor.a);
		return multChargeAlpha + (color * objColor);
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