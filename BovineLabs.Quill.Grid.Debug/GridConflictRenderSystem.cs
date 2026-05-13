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
    public unsafe partial struct GridConflictRenderSystem : ISystem
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
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridConflictRenderSystem>("Grid/Conflicts");
            if (!drawer.IsEnabled) return;

            var converter =
 new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth, singleton.GridHeight);

            foreach (var (conflicts, config) in
                     SystemAPI.Query<DynamicBuffer<GridConflictVisual>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawConflicts) continue;

                var array = conflicts.AsNativeArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var c = array[i];
                    var center = converter.CellCenter(c.Cell);
                    center.y += 0.3f;

                    var size = new float3(converter.CellSize, 0.05f, converter.CellSize);
                    drawer.Cuboid(center, quaternion.identity, size, GridPalette.Conflict, 0f);

                    var text = new FixedString64Bytes();
                    text.Append('A');
                    text.Append(c.AgentA);
                    text.Append('<');
                    text.Append('-');
                    text.Append('>');
                    text.Append('A');
                    text.Append(c.AgentB);
                    text.Append(' ');
                    text.Append('t');
                    text.Append('=');
                    text.Append(c.Time);

                    center.y += 0.4f;
                    drawer.Text64(center, text, GridPalette.Conflict, 10f, 0f);
                }
            }
        }
    }
}
#endif