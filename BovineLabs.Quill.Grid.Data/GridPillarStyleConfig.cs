using Unity.Entities;
using Unity.Mathematics;

public struct GridPillarStyleConfig : IComponentData
{
    public float3 BlockSize;
    public float Spacing;
    public float BaseHeight;
    public float TransitionSpeed;

    public float4 BaseColor;
    public float4 OutlineColor;
    public float4 HotColor;
    public float4 ColdColor;
}