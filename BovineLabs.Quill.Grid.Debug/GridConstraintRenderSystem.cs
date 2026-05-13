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
    public unsafe partial struct GridConstraintRenderSystem : ISystem
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
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridConstraintRenderSystem>("Grid/Constraints");
            if (!drawer.IsEnabled) return;

            var converter =
 new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth, singleton.GridHeight);

            foreach (var (constraints, config) in
                     SystemAPI.Query<DynamicBuffer<GridConstraintVisual>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawConstraints) continue;

                var array = constraints.AsNativeArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var c = array[i];
                    var center = converter.CellCenter(c.Cell);
                    center.y += 0.08f;

                    var color = GridPalette.AgentColor(c.Agent);
                    color.a = 0.5f;
                    var size = new float3(converter.CellSize * 0.9f, 0.03f, converter.CellSize * 0.9f);

                    drawer.Cuboid(center, quaternion.identity, size, color, 0f);

                    center.y += 0.15f;
                    drawer.Point(center, 2f, GridPalette.Constraint, 0f);
                }
            }
        }
    }
}
#endif