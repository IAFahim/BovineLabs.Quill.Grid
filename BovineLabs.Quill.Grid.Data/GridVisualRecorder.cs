using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BovineLabs.Quill.Grid.Data
{
    public static class GridVisualRecorder
    {
        public static void RecordBlockedCells(
            ref DynamicBuffer<GridBlockedData> buffer,
            in NativeArray<byte> blocked,
            int width,
            int height)
        {
            buffer.ResizeUninitialized(blocked.Length);
            for (var i = 0; i < blocked.Length; i++)
                buffer[i] = new GridBlockedData(blocked[i]);
        }

        public static void RecordPath(
            ref DynamicBuffer<GridPathVisual> buffer,
            in NativeList<int2> path)
        {
            buffer.Clear();
            for (var i = 0; i < path.Length - 1; i++)
                buffer.Add(new GridPathVisual(path[i], path[i + 1]));
        }

        public static void RecordCellLayer(
            ref DynamicBuffer<GridCellVisual> buffer,
            in NativeArray<byte> state,
            byte layer,
            int count)
        {
            for (var i = 0; i < count; i++)
                if (state[i] != 0)
                    buffer.Add(new GridCellVisual(i, state[i], layer));
        }

        public static void RecordScalarField(
            ref DynamicBuffer<GridCellVisual> buffer,
            in NativeArray<float> values,
            int count,
            byte layer)
        {
            for (var i = 0; i < count; i++)
                if (values[i] < float.PositiveInfinity)
                    buffer.Add(new GridCellVisual(i, values[i], layer));
        }
    }
}