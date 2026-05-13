#if (UNITY_EDITOR || BL_DEBUG) && (BL_GRID_FASTMARCHING || BL_GRID_EDT)
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
    public unsafe partial struct GridVectorFieldRenderSystem : ISystem
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
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridVectorFieldRenderSystem>("Grid/VectorField");
            if (!drawer.IsEnabled) return;

            var converter =
 new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth, singleton.GridHeight);
            var arrowScale = singleton.CellSize * 0.4f;

            foreach (var (vectors, config) in
                     SystemAPI.Query<DynamicBuffer<GridVectorFieldVisual>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawVectorField) continue;

                var array = vectors.AsNativeArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var v = array[i];
                    var center = converter.CellCenter(v.Cell);
                    center.y += 0.15f;

                    var dir = new float3(v.Direction.x, 0f, v.Direction.y) * arrowScale * v.Magnitude;
                    var end = center + dir;

                    drawer.Arrow(center, dir, GridPalette.VectorField, 0f);
                }
            }
        }
    }
}
#endif