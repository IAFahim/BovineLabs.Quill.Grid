using BovineLabs.Core;
using BovineLabs.Quill.Grid.Data;
using Unity.Burst;
using Unity.Entities;

#if UNITY_EDITOR || BL_DEBUG
namespace BovineLabs.Quill.Grid.Debug
{
    [WorldSystemFilter(WorldSystemFilterFlags.LocalSimulation | WorldSystemFilterFlags.ClientSimulation |
                       WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(DebugSystemGroup))]
    public partial struct GridPathRenderSystem : ISystem
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

            var drawer = SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridPathRenderSystem>("Grid/Path");
            if (!drawer.IsEnabled) return;

            var converter = new GridCoordinateConverter(singleton.Origin, singleton.CellSize, singleton.GridWidth,
                singleton.GridHeight);

            foreach (var (segments, config) in
                     SystemAPI.Query<DynamicBuffer<GridPathVisual>, GridAlgorithmVisualConfig>())
            {
                if (!config.DrawPath) continue;

                var array = segments.AsNativeArray();
                for (var i = 0; i < array.Length; i++)
                {
                    var seg = array[i];
                    var from = converter.CellCenter(seg.From.x, seg.From.y);
                    var to = converter.CellCenter(seg.To.x, seg.To.y);
                    from.y += 0.2f;
                    to.y += 0.2f;

                    drawer.Line(from, to, GridPalette.Path);
                    drawer.Point(from, 3f, GridPalette.Path);
                }
            }
        }
    }
}
#endif