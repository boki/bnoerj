// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

#include "SpriteVertexShader.vs"
#include "Shared.psh"


float4 Protanomaly(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float3 color = Normal(TextureUV).rgb;
	float3 reduced = Reduce(blindProtan, color);
	return float4(Anomylize(color, reduced.rgb), 1);
}


technique ProtanomalyTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Protanomaly();
	}
}
