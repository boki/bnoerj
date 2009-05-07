// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

#include "SpriteVertexShader.vs"
#include "Shared.psh"


float4 MinColor = float4(0, 0, 0, 1);
float4 MaxColor = float4(1.0, 1.0, 1.0, 1);
float BlackPoint = 0;
float WhitePoint = 1;

static float4 MinColorClamped = float4(0.5, 0, 0, 1);
static float4 MaxColorClamped = float4(1.0, 0, 0, 1);
static float4 ColorRange = (MaxColor - MinColor);


float4 ColorClipper(float2 TextureUV : TEXCOORD0) : COLOR
{   
	float4 color = Normal(TextureUV);
	//color = clamp(color, MinColor, MaxColor);

	float luminance = dot(color.rgb, LuminanceConv);
	//float3 scaled = color.rgb * float3(299, 587, 114);
	//float brightness = length(color);//saturate((scaled.r + scaled.g + scaled.b) / 1000);
	if (luminance < BlackPoint)
	{
		color = MinColorClamped;
	}
	else if (luminance > WhitePoint)
	{
		color = MaxColorClamped;
	}

	return color;
}


technique ColorClippingTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 ColorClipper();
	}
}
