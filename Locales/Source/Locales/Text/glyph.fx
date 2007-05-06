// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

float4 ViewportSize;
float4 TextureSize;

texture GlyphTexture;
sampler glyphSampler = sampler_state
{
   Texture = (GlyphTexture);
   ADDRESSU = CLAMP;
   ADDRESSV = CLAMP;
   MAGFILTER = NONE;
   MINFILTER = NONE;
   MIPFILTER = NONE;
};

struct VS_INPUT
{
	float4 Position   : POSITION;
	float2 TexCoord   : TEXCOORD0;
	float4 Color      : COLOR0;
};

struct VS_OUTPUT
{
	float4 Position   : POSITION;
	float2 TexCoord   : TEXCOORD0;
	float4 Color      : COLOR0;
};

VS_OUTPUT GlyphVS(VS_INPUT In)
{
	VS_OUTPUT Out;

	Out.Position.xy = In.Position.xy - 0.5;
	Out.Position.xy = Out.Position.xy / ViewportSize.xy;
	Out.Position.xy = Out.Position.xy * float2(2, -2) + float2(-1, 1);
	Out.Position.zw = In.Position.zw;	

	Out.TexCoord.xy = In.TexCoord.xy / TextureSize.xy;

	Out.Color      = In.Color;

	return Out;
}

float4 GlyphPS(VS_OUTPUT In) : COLOR0
{
	float4 color = tex2D(glyphSampler, In.TexCoord);
	//color = (1 - color.a) * In.Background + color * In.Color;
	color = color * In.Color;
    return color;
}

technique Glyph
{
	pass SinglePass
	{
		VertexShader = compile vs_2_0 GlyphVS();
		PixelShader = compile ps_2_0 GlyphPS();
	}
}
