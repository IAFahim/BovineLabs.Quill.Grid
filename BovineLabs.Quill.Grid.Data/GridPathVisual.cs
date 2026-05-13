using Unity.Entities;
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public struct GridPathVisual : IBufferElementData
    {
        public int2 From;
        public int2 To;
        public int Frame;

        public GridPathVisual(int2 from, int2 to, int frame = 0)
        {
            From = from;
            To = to;
            Frame = frame;
        }
    }
}