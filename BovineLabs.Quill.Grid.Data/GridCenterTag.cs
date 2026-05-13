using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GridCenterTag : IComponentData 
{ 
}

public struct GridVisualizerConfig : IComponentData
{
    public int2 Size;
    public float3 BlockSize;
    public float Spacing;
    public float BaseHeight;

    public float HoverRadius;
    public float HoverDepth;
        
    public float4 BaseColor;
    public float4 OutlineColor;
    public float4 HoverColorCore;
    public float4 HoverColorOuter;

    public float TransitionSpeed;

    public bool RevealEnabled;
    public float ScanPlaneYOffset;
    public float HeatmapYOffset;
    public float4 PlaneColor;
        
    public float4 HeatmapCold;
    public float4 HeatmapHot;
    public float TextSize;
}

public struct GridVisualizerInput : IComponentData
{
    public float3 HoverWorldPosition;
    public bool IsHovering;
}

public struct GridCellVisualState : IBufferElementData
{
    public float TargetDepth;
    public float CurrentDepth;
    public float4 TargetColor;
    public float4 CurrentColor;
}

