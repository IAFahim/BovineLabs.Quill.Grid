using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace BovineLabs.Quill.Grid.Authoring
{
    public class GridVisualizerAuthoring : MonoBehaviour
    {
        public int2 Size = new int2(10, 10);
        public float3 BlockSize = new float3(1f, 1f, 1f);
        public float Spacing = 0.05f;
        public float BaseHeight = 3f;

        public float HoverRadius = 4f;
        public float HoverDepth = 2.5f;
        
        public Color BaseColor = new Color(0.12f, 0.11f, 0.11f, 1f);
        public Color OutlineColor = new Color(0.25f, 0.22f, 0.22f, 1f);
        public Color HoverColorCore = new Color(1f, 0.1f, 0.1f, 0.9f);
        public Color HoverColorOuter = new Color(0.6f, 0.05f, 0.05f, 0.4f);

        public float TransitionSpeed = 12f;

        public bool RevealEnabled = true;
        public float ScanPlaneYOffset = -4f;
        public float HeatmapYOffset = -6f;
        public Color PlaneColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        
        public Color HeatmapCold = new Color(0.1f, 0.8f, 0.2f, 0.6f);
        public Color HeatmapHot = new Color(0.9f, 0.2f, 0.1f, 0.6f);
        public float TextSize = 8f;

        public bool IsCenter;
    }

    public class GridVisualizerBaker : Baker<GridVisualizerAuthoring>
    {
        public override void Bake(GridVisualizerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            
            AddComponent(entity, new GridVisualizerConfig
            {
                Size = authoring.Size,
                BlockSize = authoring.BlockSize,
                Spacing = authoring.Spacing,
                BaseHeight = authoring.BaseHeight,
                HoverRadius = authoring.HoverRadius,
                HoverDepth = authoring.HoverDepth,
                BaseColor = new float4(authoring.BaseColor.r, authoring.BaseColor.g, authoring.BaseColor.b, authoring.BaseColor.a),
                OutlineColor = new float4(authoring.OutlineColor.r, authoring.OutlineColor.g, authoring.OutlineColor.b, authoring.OutlineColor.a),
                HoverColorCore = new float4(authoring.HoverColorCore.r, authoring.HoverColorCore.g, authoring.HoverColorCore.b, authoring.HoverColorCore.a),
                HoverColorOuter = new float4(authoring.HoverColorOuter.r, authoring.HoverColorOuter.g, authoring.HoverColorOuter.b, authoring.HoverColorOuter.a),
                TransitionSpeed = authoring.TransitionSpeed,
                RevealEnabled = authoring.RevealEnabled,
                ScanPlaneYOffset = authoring.ScanPlaneYOffset,
                HeatmapYOffset = authoring.HeatmapYOffset,
                PlaneColor = new float4(authoring.PlaneColor.r, authoring.PlaneColor.g, authoring.PlaneColor.b, authoring.PlaneColor.a),
                HeatmapCold = new float4(authoring.HeatmapCold.r, authoring.HeatmapCold.g, authoring.HeatmapCold.b, authoring.HeatmapCold.a),
                HeatmapHot = new float4(authoring.HeatmapHot.r, authoring.HeatmapHot.g, authoring.HeatmapHot.b, authoring.HeatmapHot.a),
                TextSize = authoring.TextSize
            });

            AddComponent<GridVisualizerInput>(entity);
            AddBuffer<GridCellVisualState>(entity);

            if (authoring.IsCenter)
            {
                AddComponent<GridCenterTag>(entity);
            }
        }
    }
}

