
float2 ViewportSize;

// Vertex shader for rendering sprites on Windows.
void SpriteVertexShader(inout float4 position : POSITION0,
						inout float2 texCoord : TEXCOORD0)
{
	position = position;

	// Half pixel offset for correct texel centering.
	position.xy -= 0.5;

	// Viewport adjustment.
	position.xy /= ViewportSize;
	position.xy *= float2(2, -2);
	position.xy -= float2(1, -1);

	// Compute the texture coordinate.
	//texCoord /= TextureSize;
	texCoord = texCoord;
}
