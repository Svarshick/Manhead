#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture : register(t0);
sampler SpriteTextureSampler : register(s0)
{
    Texture = (SpriteTexture);
};

// Parameters set from C#
float2 GridSize;         // Grid dimensions (e.g. 100, 100)
float4 LineColor;        // Color of the grid lines
float2 CellSizeInPixels; // The size of a single cell in screen pixels (accounting for zoom)

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinate;
    
    // Coordinates inside a single cell scaled [0, 1]
    float2 cellCoord = frac(uv * GridSize);
    
    // Map the relative cell coordinate to actual screen pixels
    float2 pixelCoord = cellCoord * CellSizeInPixels;
    
    // Find distance to the closest cell boundary in pixels
    float2 distToBorder = min(pixelCoord, CellSizeInPixels - pixelCoord);
    
    // Anti-aliased line rendering (1.5px total thickness)
    float2 gridLine = smoothstep(1.25, 0.25, distToBorder);
    float intensity = max(gridLine.x, gridLine.y);
    
    // Calculate boundaries around the entire outer field bounds
    float2 boundaryCoord = uv * GridSize * CellSizeInPixels;
    float2 distToBoundary = min(boundaryCoord, (GridSize * CellSizeInPixels) - boundaryCoord);
    float2 boundaryLine = smoothstep(1.25, 0.25, distToBoundary);
    
    intensity = max(intensity, max(boundaryLine.x, boundaryLine.y));
    
    return LineColor * intensity;
}

technique GridDrawing
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
};