using BovineLabs.Grid;
using Unity.Mathematics;
using Unity.Collections;
using BovineLabs.Quill;
using FireAlt.BLinq;

public static class GridMathPrimitives
{
    public static bool TryGetLocalPosition(int2 xy, int2 size, float3 blockSize, float spacing, out float3 localPos)
    {
        float2 offset = (new float2(size.x, size.y) - 1f) * 0.5f;
        float2 p = (new float2(xy.x, xy.y) - offset) * (blockSize.xz + spacing);
        localPos = new float3(p.x, 0f, p.y);
        return true;
    }

    public static bool TryGetUV(int2 xy, int2 size, out float2 uv)
    {
        uv = new float2(xy.x, xy.y) / math.max(new float2(size.x - 1, size.y - 1), 1f);
        return true;
    }
}

public static class GridVisualPrimitives
{
    public static bool TryCalculateHoverTarget(
        float3 worldPos, 
        float3 hoverPos, 
        bool isHovering, 
        float radius, 
        float maxDepth, 
        float4 defaultColor, 
        float4 hoverColor, 
        out float targetDepth, 
        out float4 targetColor)
    {
        if (!isHovering)
        {
            targetDepth = 0f;
            targetColor = defaultColor;
            return true;
        }

        float dist = math.distance(worldPos.xz, hoverPos.xz);
        float t = math.saturate(1f - (dist / radius));
        
        targetDepth = math.lerp(0f, maxDepth, t);
        targetColor = math.lerp(defaultColor, hoverColor, t);
        return true;
    }

    public static bool TryStepState(in GridCellVisualState current, float targetDepth, float4 targetColor, float dt, float speed, out GridCellVisualState next)
    {
        float t = math.saturate(dt * speed);
        next = new GridCellVisualState
        {
            TargetDepth = targetDepth,
            TargetColor = targetColor,
            CurrentDepth = math.lerp(current.CurrentDepth, targetDepth, t),
            CurrentColor = math.lerp(current.CurrentColor, targetColor, t)
        };
        return true;
    }
}

public static class GridQuillPrimitives
{
    public static bool TryDrawBlock(this ref Drawer drawer, float3 center, float3 size, float depth, float4 color)
    {
        float3 pos = center + new float3(0f, -depth, 0f);
        var col = new UnityEngine.Color(color.x, color.y, color.z, color.w);
        
        drawer.Cuboid(pos, quaternion.identity, size, col);
        return true;
    }

    public static bool TryDrawReveal(this ref Drawer drawer, float3 center, float2 size, float yOffset, float4 color, float2 infoData, float textSize)
    {
        float3 pos = center + new float3(0f, yOffset, 0f);
        var col = new UnityEngine.Color(color.x, color.y, color.z, color.w);
        
        float3 p0 = pos + new float3(-size.x, 0f, -size.y) * 0.5f;
        float3 p1 = pos + new float3(-size.x, 0f,  size.y) * 0.5f;
        float3 p2 = pos + new float3( size.x, 0f,  size.y) * 0.5f;
        float3 p3 = pos + new float3( size.x, 0f, -size.y) * 0.5f;

        drawer.Line(p0, p1, col);
        drawer.Line(p1, p2, col);
        drawer.Line(p2, p3, col);
        drawer.Line(p3, p0, col);
        drawer.Line(p0, p2, col); 

        FixedString32Bytes text = default;
        text.Append("u: ");
        FormatFloatFast(ref text, infoData.x);
        text.Append(", v: ");
        FormatFloatFast(ref text, infoData.y);

        drawer.Text32(pos, text, col, textSize);
        return true;
    }

    private static void FormatFloatFast(ref FixedString32Bytes str, float val)
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
    }
}

public static class GridBLinqPrimitives
{
    public static GridCellVisualState ComputeNextState(
        this IndexTuple<GridCellVisualState> item, 
        in GridVisualizerConfig config, 
        in GridVisualizerInput input, 
        float3 origin, 
        float dt)
    {
        var grid = Grid2D.Create(config.Size.x, config.Size.y);
        int2 xy = grid.ToCoord(item.Index);
        
        GridMathPrimitives.TryGetLocalPosition(xy, config.Size, config.BlockSize, config.Spacing, out float3 localPos);
        float3 worldPos = origin + localPos;

        GridVisualPrimitives.TryCalculateHoverTarget(
            worldPos, 
            input.HoverWorldPosition, 
            input.IsHovering, 
            config.HoverRadius, 
            config.HoverDepth, 
            config.DefaultColor, 
            config.HoverColor, 
            out float targetDepth, 
            out float4 targetColor);
            
        GridVisualPrimitives.TryStepState(in item.Item, targetDepth, targetColor, dt, config.TransitionSpeed, out GridCellVisualState next);
        
        return next;
    }
}