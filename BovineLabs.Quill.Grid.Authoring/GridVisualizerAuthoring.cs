using BovineLabs.Quill.Grid.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace BovineLabs.Quill.Grid.Authoring
{
    public class GridVisualizerAuthoring : MonoBehaviour
    {
        public float cellSize = 1f;
        public int gridWidth = 32;
        public int gridHeight = 32;
        public Vector3 origin = Vector3.zero;
        public bool enabledByDefault = true;

        public string algorithmName = "Grid";
        public string category = "Grid";

        [Header("Visualization Toggles")] public bool drawGrid = true;

        public bool drawObstacles = true;
        public bool drawFrontier = true;
        public bool drawClosed = true;
        public bool drawPath = true;
        public bool drawLabels = true;
        public bool drawHeatmap = true;
        public bool drawIntervals;
        public bool drawConstraints;
        public bool drawConflicts;
        public bool drawMessages;
        public bool drawVectorField;
        public bool drawTimeline;
    }

    public class GridVisualizerBaker : Baker<GridVisualizerAuthoring>
    {
        public override void Bake(GridVisualizerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new GridVisualizerSingleton
            {
                Enabled = authoring.enabledByDefault,
                CellSize = authoring.cellSize,
                GridWidth = authoring.gridWidth,
                GridHeight = authoring.gridHeight,
                Origin = authoring.origin,
                Mode = GridVisualizerMode.Live,
                MaxFrames = 256
            });

            AddComponent(entity, new GridAlgorithmVisualConfig
            {
                AlgorithmName = new FixedString64Bytes(authoring.algorithmName),
                Category = new FixedString32Bytes(authoring.category),
                DrawGrid = authoring.drawGrid,
                DrawObstacles = authoring.drawObstacles,
                DrawFrontier = authoring.drawFrontier,
                DrawClosed = authoring.drawClosed,
                DrawPath = authoring.drawPath,
                DrawLabels = authoring.drawLabels,
                DrawHeatmap = authoring.drawHeatmap,
                DrawIntervals = authoring.drawIntervals,
                DrawConstraints = authoring.drawConstraints,
                DrawConflicts = authoring.drawConflicts,
                DrawMessages = authoring.drawMessages,
                DrawVectorField = authoring.drawVectorField,
                DrawTimeline = authoring.drawTimeline
            });

            AddBuffer<GridCellVisual>(entity);
            AddBuffer<GridLineVisual>(entity);
            AddBuffer<GridTextVisual>(entity);
#if BL_GRID_ANYA
            AddBuffer<GridIntervalVisual>(entity);
#endif
#if BL_GRID_BELIEF
            AddBuffer<GridArrowVisual>(entity);
#endif
            AddBuffer<GridPathVisual>(entity);
#if BL_GRID_CBS
            AddBuffer<GridAgentPathVisual>(entity);
            AddBuffer<GridConflictVisual>(entity);
            AddBuffer<GridConstraintVisual>(entity);
#endif
#if BL_GRID_FASTMARCHING || BL_GRID_EDT
            AddBuffer<GridVectorFieldVisual>(entity);
#endif
            AddBuffer<GridBlockedData>(entity);
        }
    }
}