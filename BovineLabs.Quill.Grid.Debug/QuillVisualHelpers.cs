using BovineLabs.Quill;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace BovineLabs.Quill.Grid.Debug
{
    public static class QuillVisualHelpers
    {
        public static bool TryDrawTechPillar(
            ref this Drawer drawer,
            float3 basePos,
            float3 size,
            float baseHeight,
            float carveDepth,
            Color fillColor,
            Color outlineColor)
        {
            float currentHeight = math.max(0.01f, baseHeight - carveDepth);
            float3 extents = new float3(size.x, 0f, size.z) * 0.5f;

            float3 b0 = basePos + new float3(-extents.x, 0f, -extents.z);
            float3 b1 = basePos + new float3(-extents.x, 0f, extents.z);
            float3 b2 = basePos + new float3(extents.x, 0f, extents.z);
            float3 b3 = basePos + new float3(extents.x, 0f, -extents.z);

            float3 t0 = b0 + new float3(0f, currentHeight, 0f);
            float3 t1 = b1 + new float3(0f, currentHeight, 0f);
            float3 t2 = b2 + new float3(0f, currentHeight, 0f);
            float3 t3 = b3 + new float3(0f, currentHeight, 0f);

            // Tech shading
            Color topCol = fillColor;
            Color frontCol = new Color(fillColor.r * 0.6f, fillColor.g * 0.6f, fillColor.b * 0.6f, fillColor.a);
            Color rightCol = new Color(fillColor.r * 0.3f, fillColor.g * 0.3f, fillColor.b * 0.3f, fillColor.a);
            Color backCol = new Color(fillColor.r * 0.4f, fillColor.g * 0.4f, fillColor.b * 0.4f, fillColor.a);
            Color leftCol = new Color(fillColor.r * 0.5f, fillColor.g * 0.5f, fillColor.b * 0.5f, fillColor.a);

            // Faces
            drawer.SolidQuad(t0, t1, t2, t3, topCol);
            drawer.SolidQuad(b0, t0, t3, b3, frontCol);
            drawer.SolidQuad(b3, t3, t2, b2, rightCol);
            drawer.SolidQuad(b2, t2, t1, b1, backCol);
            drawer.SolidQuad(b1, t1, t0, b0, leftCol);

            // Tech bevel lines on top
            drawer.Line(t0, t1, outlineColor);
            drawer.Line(t1, t2, outlineColor);
            drawer.Line(t2, t3, outlineColor);
            drawer.Line(t3, t0, outlineColor);

            return true;
        }

        public static bool TryDrawScanPlaneGlobal(
            ref this Drawer drawer,
            float3 center,
            float2 totalSize,
            Color color)
        {
            float3 extents = new float3(totalSize.x, 0f, totalSize.y) * 0.5f;
            
            float3 p0 = center + new float3(-extents.x, 0f, -extents.z); // BL
            float3 p1 = center + new float3(-extents.x, 0f, extents.z);  // TL
            float3 p2 = center + new float3(extents.x, 0f, extents.z);   // TR
            float3 p3 = center + new float3(extents.x, 0f, -extents.z);  // BR

            // Dotted edges (faked with 4 corners for now to keep clean, or segmented lines)
            drawer.Line(p0, p1, color);
            drawer.Line(p1, p2, color);
            drawer.Line(p2, p3, color);
            drawer.Line(p3, p0, color);

            // Corner 'X' markers
            float m = 0.5f;
            DrawX(ref drawer, p0, m, color);
            DrawX(ref drawer, p1, m, color);
            DrawX(ref drawer, p2, m, color);
            DrawX(ref drawer, p3, m, color);

            return true;
        }

        private static void DrawX(ref Drawer drawer, float3 pos, float size, Color col)
        {
            drawer.Line(pos + new float3(-size, 0f, -size), pos + new float3(size, 0f, size), col);
            drawer.Line(pos + new float3(-size, 0f, size), pos + new float3(size, 0f, -size), col);
        }

        public static bool TryDrawScanLineCell(
            ref this Drawer drawer,
            float3 cellCenter,
            float3 blockSize,
            Color color)
        {
            float3 extents = new float3(blockSize.x, 0f, blockSize.z) * 0.5f;
            float3 left = cellCenter + new float3(-extents.x, 0f, 0f);
            float3 right = cellCenter + new float3(extents.x, 0f, 0f);
            
            drawer.Line(left, right, color);
            return true;
        }

        public static bool TryDrawHeatmapCell(
            ref this Drawer drawer,
            float3 center,
            float3 size,
            float dataValue,
            Color tileColor,
            Color textColor,
            float textSize)
        {
            float3 extents = new float3(size.x, 0f, size.z) * 0.5f;
            
            float3 p0 = center + new float3(-extents.x, 0f, -extents.z);
            float3 p1 = center + new float3(-extents.x, 0f, extents.z);
            float3 p2 = center + new float3(extents.x, 0f, extents.z);
            float3 p3 = center + new float3(extents.x, 0f, -extents.z);

            drawer.SolidQuad(p0, p1, p2, p3, tileColor);

            TryFormatDecimalText(dataValue, out FixedString32Bytes text);
            quaternion flatRotation = quaternion.Euler(math.PI / 2f, 0f, 0f);
            drawer.Text32(center + new float3(0f, 0.05f, 0f), flatRotation, text, textColor, textSize);

            return true;
        }

        public static bool TryDrawCursor(
            ref this Drawer drawer,
            float3 position,
            Color color)
        {
            float h = 1.5f;
            float r = 0.5f;
            float3 top = position + new float3(0f, h, 0f);
            
            float3 b0 = position + new float3(-r, 0f, -r);
            float3 b1 = position + new float3(-r, 0f, r);
            float3 b2 = position + new float3(r, 0f, r);
            float3 b3 = position + new float3(r, 0f, -r);

            drawer.Line(b0, b1, color);
            drawer.Line(b1, b2, color);
            drawer.Line(b2, b3, color);
            drawer.Line(b3, b0, color);

            drawer.Line(top, b0, color);
            drawer.Line(top, b1, color);
            drawer.Line(top, b2, color);
            drawer.Line(top, b3, color);
            
            return true;
        }

        public static bool TryDrawUVText(
            ref this Drawer drawer,
            float3 position,
            quaternion rotation,
            float2 uv,
            Color color,
            float textSize)
        {
            TryFormatUVText(uv, out FixedString32Bytes text);
            drawer.Text32(position, rotation, text, color, textSize);
            return true;
        }

        public static bool TryFormatUVText(
            float2 uv,
            out FixedString32Bytes result)
        {
            result = default;
            result.Append("u: ");
            TryFormatFloatFast(uv.x, ref result);
            result.Append(", v: ");
            TryFormatFloatFast(uv.y, ref result);
            return true;
        }

        public static bool TryFormatDecimalText(float val, out FixedString32Bytes result)
        {
            result = default;
            result.Append('.');
            int frac = (int)(math.abs(val) * 100f) % 100;
            if (frac < 10) result.Append('0');
            result.Append(frac);
            return true;
        }

        public static bool TryFormatFloatFast(float val, ref FixedString32Bytes str)
        {
            if (val < 0)
            {
                str.Append('-');
                val = -val;
            }

            int whole = (int)val;
            int frac = (int)(math.abs(val - whole) * 100f);
            str.Append(whole);
            str.Append('.');
            if (frac < 10) str.Append('0');
            str.Append(frac);
            
            return true;
        }
    }
}