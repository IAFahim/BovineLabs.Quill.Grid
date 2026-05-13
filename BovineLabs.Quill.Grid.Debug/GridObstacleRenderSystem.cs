using BovineLabs.Core;
using BovineLabs.Quill.Grid.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

#if UNITY_EDITOR || BL_DEBUG
namespace BovineLabs.Quill.Grid.Debug
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation |
                       WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(DebugSystemGroup))]
    public partial struct GridObstacleRenderSystem : ISystem
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
                .CreateDrawer<GridObstacleRenderSystem>("Grid/Obstacles");
            if (!drawer.IsEnabled) return;

            var converter = new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth,
                singleton.GridHeight);

            foreach (var (blocked, config) in
                     SystemAPI.Query<DynamicBuffer<GridBlockedData>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawObstacles) continue;

                var array = blocked.AsNativeArray();
                var size = new float3(converter.CellSize * 0.98f, 0.1f, converter.CellSize * 0.98f);

                for (var i = 0; i < array.Length; i++)
                {
                    if (array[i].Value == 0) continue;

                    var center = converter.CellCenter(i);
                    center.y += 0.05f;
                    drawer.Cuboid(center, quaternion.identity, size, GridPalette.Obstacle);
                }
            }
        }
    }
}
#endif