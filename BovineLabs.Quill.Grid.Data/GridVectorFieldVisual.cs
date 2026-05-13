#if BL_GRID_FASTMARCHING || BL_GRID_EDT
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public struct GridVectorFieldVisual : IBufferElementData
    {
        public int Cell;
        public float2 Direction;
        public float Magnitude;

        public GridVectorFieldVisual(int cell, float2 direction, float magnitude)
        {
            Cell = cell;
            Direction = direction;
            Magnitude = magnitude;
        }
    }
}
#endif