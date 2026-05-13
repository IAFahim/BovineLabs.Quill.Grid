#if (UNITY_EDITOR || BL_DEBUG) && BL_GRID_CBS
namespace BovineLabs.Quill.Grid.Debug
{
    using BovineLabs.Quill;
    using BovineLabs.Core;
    using BovineLabs.Quill.Grid.Data;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(DebugSystemGroup))]
    public unsafe partial struct GridAgentPathRenderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GridVisualizerSingleton>();
            state.RequireForUpdate<DrawSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI.GetSingleton<GridVisualizerSingleton>();
            if (!singleton.Enabled) return;

            var drawer =
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridAgentPathRenderSystem>("Grid/AgentPaths");
            if (!drawer.IsEnabled) return;

            var converter =
 new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth, singleton.GridHeight);

            foreach (var (agents, config) in
                     SystemAPI.Query<DynamicBuffer<GridAgentPathVisual>, GridAlgorithmVisualConfig>())
            {
                var array = agents.AsNativeArray();
                for (int i = 1; i < array.Length; i++)
                {
                    var prev = array[i - 1];
                    var curr = array[i];

                    if (prev.AgentIndex != curr.AgentIndex) continue;

                    var from = converter.CellCenter(prev.Cell);
                    var to = converter.CellCenter(curr.Cell);
                    from.y += 0.25f;
                    to.y += 0.25f;

                    var color = GridPalette.AgentColor(curr.AgentIndex);
                    drawer.Line(from, to, color, 0f);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    var a = array[i];
                    var pos = converter.CellCenter(a.Cell);
                    pos.y += 0.25f;
                    var color = GridPalette.AgentColor(a.AgentIndex);
                    drawer.Point(pos, 3f, color, 0f);
                }
            }
        }
    }
}
#endif