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

static float4 ColorRange = (MaxColor - MinColor);


float4 ColorStretcher(float2 TextureUV : TEXCOORD0) : COLOR
{   
	float4 color = Normal(TextureUV);
	color = clamp(color, MinColor, MaxColor);
	color = (color - MinColor) / (MaxColor - MinColor);
	color.a = 1;
	return color;
}

technique ColorStretchingTechnique
{
	pass P0
    {
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader  = compile ps_2_0 ColorStretcher();
	}
}
