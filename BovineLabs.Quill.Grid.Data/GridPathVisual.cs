using Unity.Entities;
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public struct GridPathVisual : IBufferElementData
    {
        public int2 From;
        public int2 To;

        public GridPathVisual(int2 from, int2 to)
        {
            From = from;
            To = to;
        }
    }
}