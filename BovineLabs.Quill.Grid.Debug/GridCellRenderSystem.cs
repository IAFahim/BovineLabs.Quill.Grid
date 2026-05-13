using BovineLabs.Core;
using BovineLabs.Quill.Grid.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR || BL_DEBUG
namespace BovineLabs.Quill.Grid.Debug
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation |
                       WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(DebugSystemGroup))]
    public partial struct GridCellRenderSystem : ISystem
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

            var drawer = SystemAPI.GetSingleton<DrawSystem.Singleton>()
                .CreateDrawer<GridCellRenderSystem>("Grid/Cells");
            if (!drawer.IsEnabled) return;

            var converter = new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth,
                singleton.GridHeight);

            foreach (var (cells, config) in
                     SystemAPI.Query<DynamicBuffer<GridCellVisual>, GridAlgorithmVisualConfig>())
            {
                var array = cells.AsNativeArray();
                for (var i = 0; i < array.Length; i++)
                {
                    var cell = array[i];
                    var center = converter.CellCenter(cell.Cell);
                    center.y += 0.05f;

                    var color = SelectColor(cell.Layer, cell.Value);
                    var size = new float3(converter.CellSize * 0.95f, 0.05f, converter.CellSize * 0.95f);
                    drawer.Cuboid(center, quaternion.identity, size, color);
                }
            }
        }

        private static Color SelectColor(byte layer, float value)
        {
            switch (layer)
            {
                case GridCellVisual.LayerObstacle: return GridPalette.Obstacle;
                case GridCellVisual.LayerFrontier: return GridPalette.Frontier;
                case GridCellVisual.LayerClosed: return GridPalette.Closed;
                case GridCellVisual.LayerPath: return GridPalette.Path;
                case GridCellVisual.LayerHeatmap: return GridPalette.LerpHeatmap(value);
                case GridCellVisual.LayerConflict: return GridPalette.Conflict;
                case GridCellVisual.LayerConstraint: return GridPalette.Constraint;
                default: return Color.magenta;
            }
        }
    }
}
#endif