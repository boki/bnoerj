// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

//
// Global variables
//

static float3 Gamma = float3(2.2, 2.2, 2.2);
static float3 InvGamma = float3(1 / 2.2, 1 / 2.2, 1 / 2.2);
static float3 LuminanceConv = { 0.299f, 0.587f, 0.114f };

texture Texture;
sampler TextureSampler = sampler_state
{
	Texture = <Texture>;
	MipFilter = NONE;
	MinFilter = NONE;
	MagFilter = NONE;
};

#include "SpriteVertexShader.vs"

//
// Pixel Shaders
//

float3x3 RGB2XYZ =
{
	0.4306, 0.3416, 0.1783,
	0.2220, 0.7067, 0.0713,
	0.0202, 0.1296, 0.9392
};
float3x3 XYZ2RGB =
{
	 3.0632, -1.3933, -0.4758, 
	-0.9692,  1.8760,  0.0416, 
	 0.0679, -0.2289,  1.0693 
};

// white point xyz coords:
float3 w = { 0.312713, 0.329016, 0.358271 };

struct Blind
{
	float cpu; // confusion point, u coord
	float cpv; // confusion point, v coord
	float abu; // color axis begining point, u coord
	float abv; // color axis begining point, v coord
	float aeu; // color axis ending point, u coord
	float aev; // color axis ending point, v coord
	float am;
	float ayi;
};
Blind blindProtan =
{
	0.735,
	0.265,
	0.115807,
	0.073581,
	0.471899,
	0.527051,
	1.273463,
	-0.073894
};
Blind blindDeutan =
{
	1.14,
	-0.14,
	0.102776,
	0.102864,
	0.505845,
	0.493211,
	0.968437,
	0.003331
};
Blind blindTritan =
{
	0.171,
	-0.003,
	0.045391,
	0.294976,
	0.665764,
	0.334011,
	0.062921,
	0.292119
};

float3 Reduce(Blind blind, float3 rgb)
{
	// map RGB input into XYZ space...
	rgb = pow(rgb, Gamma);
	float3 c = mul(RGB2XYZ, rgb);

	// map into uvY space...
	float sumXYZ = c.x + c.y + c.z;
	float2 uvY = 0;
	if (sumXYZ != 0)
	{
		uvY = c.xy / sumXYZ;
	}

	// find neutral grey at this luminosity (we keep the same Y value)
	float3 n = w * c.y / w.y;

	// cl is "confusion line" between our color and the confusion point
	// clm is cl's slope, and clyi is cl's "y-intercept" (actually on the "v" axis at u=0)
	float clm;
	if (uvY.x < blind.cpu)
	{
		clm = (blind.cpv - uvY.y) / (blind.cpu - uvY.x);
	}
	else
	{
		clm = (uvY.y - blind.cpv) / (uvY.x - blind.cpu);
	}
	float clyi = uvY.y - uvY.x * clm;

	// find the change in the u and v dimensions (no Y change)
	float2 duv;
	duv.x = (blind.ayi - clyi) / (clm - blind.am);
	duv.y = (clm * duv.x) + clyi;

	// find the simulated color's XYZ coords
	float cydy = c.y / duv.y;
	float3 s = float3(duv.x * cydy, c.y, (1 - (duv.x + duv.y)) * cydy);

	// note the RGB differences between sim color and our neutral color
	float3 d = n - s;
	d.y = 0;

	// and then try to plot the RGB coords
	s = mul(XYZ2RGB, s);
	d = mul(XYZ2RGB, d);

	// find out how much to shift sim color toward neutral to fit in RGB space:
	float3 adj = 1 - /*abs(s);*/float3(
		((s.r < 0 ? 0 : 1) - s.r),
		((s.g < 0 ? 0 : 1) - s.g),
		((s.b < 0 ? 0 : 1) - s.b));
	adj.r = d.r ? adj.r / d.r : 0;
	adj.g = d.g ? adj.g / d.g : 0;
	adj.b = d.b ? adj.b / d.b : 0;
	adj.r = adj.r > 1 ? 0 : adj.r;
	adj.g = adj.g > 1 ? 0 : adj.g;
	adj.b = adj.b > 1 ? 0 : adj.b;

	float adjust = max(0, adj.r);
	adjust = max(adjust, adj.g);
	adjust = max(adjust, adj.b);

	// now shift *all* three proportional to the greatest shift...
	s += (adjust * d);

	// and then try to plot the RGB coords
	s = mul(XYZ2RGB, s);

	// and then return the resulting simulated color...
	return pow(s, InvGamma);
}


/*
v seemed like an interesting weight ratio. No idea about accuracy though.
To do this right, it should happen in yuv (or similar) color space. However,
with no sense of what would be accurate, there's little point in doing it
the 'right way'. Anomylous vision types can fall anywhere in a broad range
between normal vision and some form of color blindness. We'll never be
correct for every case, so we'll just do the math the quick way...
*/
float AnomylizeV = 1.75;
float AnomylizeVd = 1 / (1.75 * 1 + 1);
float3 Anomylize(float3 original, float3 filtered)
{
	return (AnomylizeV * filtered + original) * AnomylizeVd;
}


float4 Normal(float2 TextureUV : TEXCOORD0) : COLOR0
{
	return tex2D(TextureSampler, TextureUV);
}

float4 ColorBrightness(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float4 color = Normal(TextureUV);
	color.rgb *= float3(299, 587, 114);
	color.rgb = saturate((color.r * color.g + color.b) / 1000);
	return color;
}

float4 ReducePS(uniform Blind blind, float2 TextureUV : TEXCOORD0) : COLOR0
{
	float4 color = Normal(TextureUV);
	color.rgb = Reduce(blind, color.rgb);
	return color;
}

float4 Monoch(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float4 color = Normal(TextureUV);
	color.rgb = dot(color.rgb, LuminanceConv);
	return color;
}

float4 Protanomaly(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float3 color = Normal(TextureUV).rgb;
	float3 reduced = Reduce(blindProtan, color);
	return float4(Anomylize(color, reduced.rgb), 1);
}

float4 Deutanomaly(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float3 color = Normal(TextureUV).rgb;
	float3 reduced = Reduce(blindDeutan, color);
	return float4(Anomylize(color, reduced.rgb), 1);
}

float4 Tritanomaly(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float3 color = Normal(TextureUV).rgb;
	float3 reduced = Reduce(blindTritan, color);
	return float4(Anomylize(color, reduced.rgb), 1);
}

float4 Anommo(float2 TextureUV : TEXCOORD0) : COLOR0
{
	float4 color = Normal(TextureUV);

    float v = 4; // seemed like an interesting weight. No idea about accuracy.
    float d = v + 1; // anom's can vary widely, so don't worry too much about it.
    float m = color.r;
    float3 anommo = float3(
			(m * v + color.r) / d,
			(m * v + color.g) / d,
			(m * v + color.b) / d
		);
	color.rgb = anommo;
	return color;
}

//
// Techniques
//

technique ColorBrightnessTechnique
{
	pass P0
	{          
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 ColorBrightness();
	}
}

technique ProtanopiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 ReducePS(blindProtan);
	}
}

technique DeutanopiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 ReducePS(blindDeutan);
	}
}

technique TritanopiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 ReducePS(blindTritan);
	}
}

technique TypicalAchromatopsiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Monoch();
	}
}

technique ProtanomalyTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Protanomaly();
	}
}

technique DeutanomalyTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Deutanomaly();
	}
}

technique TritanomalyTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Tritanomaly();
	}
}

technique AtypicalAchromatopsiaTechnique
{
	pass P0
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader  = compile ps_3_0 Anommo();
	}
}
