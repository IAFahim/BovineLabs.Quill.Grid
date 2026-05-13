#if (UNITY_EDITOR || BL_DEBUG) && BL_GRID_ANYA
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
    public unsafe partial struct GridIntervalRenderSystem : ISystem
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
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridIntervalRenderSystem>("Grid/Intervals");
            if (!drawer.IsEnabled) return;

            var converter =
 new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth, singleton.GridHeight);

            foreach (var (intervals, config) in
                     SystemAPI.Query<DynamicBuffer<GridIntervalVisual>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawIntervals) continue;

                var array = intervals.AsNativeArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var iv = array[i];

                    var left = converter.CellCenter((int)iv.XL, iv.Y);
                    var right = converter.CellCenter((int)iv.XR, iv.Y);
                    left.y += 0.1f;
                    right.y += 0.1f;

                    var color = iv.IsExpanded ? GridPalette.IntervalExpanded : GridPalette.Interval;
                    drawer.Line(left, right, color, 0f);

                    var root3d = new float3(
                        converter.Origin.x + iv.Root.x * converter.CellSize + converter.CellHalfSize,
                        0.15f,
                        converter.Origin.z + iv.Root.y * converter.CellSize + converter.CellHalfSize);
                    drawer.Point(root3d, 4f, GridPalette.RootPoint, 0f);

                    var mid = (left + right) * 0.5f;
                    mid.y += 0.2f;
                    drawer.Line(root3d, mid, new Color(color.r, color.g, color.b, 0.3f), 0f);
                }
            }
        }
    }
}
#endif