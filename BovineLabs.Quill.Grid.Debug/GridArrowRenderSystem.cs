#if (UNITY_EDITOR || BL_DEBUG) && BL_GRID_BELIEF
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
    public unsafe partial struct GridArrowRenderSystem : ISystem
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
 SystemAPI.GetSingleton<DrawSystem.Singleton>().CreateDrawer<GridArrowRenderSystem>("Grid/Arrows");
            if (!drawer.IsEnabled) return;

            foreach (var arrows in SystemAPI.Query<DynamicBuffer<GridArrowVisual>>())
            {
                var array = arrows.AsNativeArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var a = array[i];
                    var color = new Color(a.Color.x, a.Color.y, a.Color.z, a.Color.w);
                    drawer.Arrow(a.From, a.To - a.From, color, 0f);
                }
            }
        }
    }
}
#endif