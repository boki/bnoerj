// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

#include "SpriteVertexShader.vs"
#include "Shared.psh"


float4 Monoch(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float4 color = Normal(TextureUV);
	color.rgb = dot(color.rgb, LuminanceConv);
	return color;
}


technique TypicalAchromatopsiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader  = compile ps_2_0 Monoch();
	}
}
