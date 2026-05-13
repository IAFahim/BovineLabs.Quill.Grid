#if BL_GRID_ANYA
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public struct GridIntervalVisual : IBufferElementData
    {
        public int Y;
        public float XL;
        public float XR;
        public float2 Root;
        public float Cost;
        public int ParentIndex;
        public bool IsExpanded;

        public GridIntervalVisual(int y, float xl, float xr, float2 root, float cost, int parentIndex, bool isExpanded)
        {
            Y = y;
            XL = xl;
            XR = xr;
            Root = root;
            Cost = cost;
            ParentIndex = parentIndex;
            IsExpanded = isExpanded;
        }
    }
}
#endif