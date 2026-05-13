using Unity.Entities;

public struct GridVisualizerStyle : IComponentData
{
    public GridVisualStyleType Type;
}

public enum GridVisualStyleType : byte
{
    Flat,
    Pillar,
    HeatmapPlane,
    ScanReveal,
    FlowField,
    CombatThreat
}