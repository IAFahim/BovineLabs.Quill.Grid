using BovineLabs.Core;
using BovineLabs.Quill.Grid.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace BovineLabs.Quill.Grid.Debug
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation |
                       WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(DebugSystemGroup))]
    public partial struct GridAgentPathRenderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridVisualizerData>();
            state.RequireForUpdate<DrawSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var drawer =
                SystemAPI.GetSingleton<DrawSystem.Singleton>()
                    .CreateDrawer<GridAgentPathRenderSystem>("Grid/AgentPaths");
            if (!drawer.IsEnabled) return;

            foreach (var (visualizer, global, config, agents) in
                     SystemAPI.Query<GridVisualizerData, GridVisualizerGlobal, GridAlgorithmVisualConfig,
                         DynamicBuffer<GridAgentPathVisual>>())
            {
                if (!global.Enabled) continue;
                if (!config.DrawPath) continue;

                var converter =
                    new GridCoordinateConverter(visualizer.Origin, visualizer.CellSize, visualizer.GridWidth,
                        visualizer.GridHeight);

                var array = agents.AsNativeArray();
                for (var i = 0; i < array.Length; i++)
                {
                    var curr = array[i];
                    var prevIndex = FindStep(array, curr.AgentIndex, curr.TimeStep - 1);
                    if (prevIndex < 0) continue;

                    var prev = array[prevIndex];

                    if (global.Mode == GridVisualizerMode.Step && curr.TimeStep > global.CurrentFrame) continue;

                    var from = converter.CellCenter(prev.Cell);
                    var to = converter.CellCenter(curr.Cell);
                    from.y += 0.25f;
                    to.y += 0.25f;

                    var color = GridPalette.AgentColor(curr.AgentIndex);
                    if (config.DrawTimeline && curr.TimeStep != global.CurrentFrame)
                        color.a = 0.25f;

                    drawer.Line(from, to, color);
                }

                for (var i = 0; i < array.Length; i++)
                {
                    var a = array[i];
                    if (global.Mode == GridVisualizerMode.Step && a.TimeStep > global.CurrentFrame) continue;

                    var pos = converter.CellCenter(a.Cell);
                    pos.y += 0.25f;
                    var color = GridPalette.AgentColor(a.AgentIndex);
                    if (config.DrawTimeline && a.TimeStep != global.CurrentFrame)
                        color.a = 0.35f;

                    drawer.Point(pos, 3f, color);
                }
            }
        }

        private static int FindStep(NativeArray<GridAgentPathVisual> array, int agentIndex, int timeStep)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var path = array[i];
                if (path.AgentIndex == agentIndex && path.TimeStep == timeStep)
                    return i;
            }

            return -1;
        }
    }
}