#if BL_GRID_CBS
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public struct GridAgentPathVisual : IBufferElementData
    {
        public int AgentIndex;
        public int TimeStep;
        public int Cell;

        public GridAgentPathVisual(int agentIndex, int timeStep, int cell)
        {
            AgentIndex = agentIndex;
            TimeStep = timeStep;
            Cell = cell;
        }
    }
}
#endif