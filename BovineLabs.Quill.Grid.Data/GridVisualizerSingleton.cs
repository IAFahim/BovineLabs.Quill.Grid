using Unity.Entities;
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public enum GridVisualizerMode : byte
    {
        Live,
        Step,
        Snapshot
    }

    public struct GridVisualizerSingleton : IComponentData
    {
        public bool Enabled;
        public int CurrentFrame;
        public int MaxFrames;
        public float CellSize;
        public float3 Origin;
        public GridVisualizerMode Mode;
        public int GridWidth;
        public int GridHeight;
    }
}