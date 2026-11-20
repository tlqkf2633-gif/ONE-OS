namespace OneOS.Core;

[Flags]
public enum ResizeEdges
{
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8
}

public readonly record struct WindowBounds(double Left, double Top, double Width, double Height);

public static class WindowGeometry
{
    public static WindowBounds Move(WindowBounds bounds, double dx, double dy, double workspaceWidth, double workspaceHeight, double visibleGrip = 80)
    {
        var maxLeft = Math.Max(0, workspaceWidth - visibleGrip);
        var maxTop = Math.Max(0, workspaceHeight - visibleGrip);
        return bounds with
        {
            Left = Math.Clamp(bounds.Left + dx, 0, maxLeft),
            Top = Math.Clamp(bounds.Top + dy, 0, maxTop)
        };
    }

    public static WindowBounds Resize(WindowBounds bounds, ResizeEdges edges, double dx, double dy,
        double workspaceWidth, double workspaceHeight, double minWidth, double minHeight)
    {
        var left = bounds.Left;
        var top = bounds.Top;
        var width = bounds.Width;
        var height = bounds.Height;

        if ((edges & ResizeEdges.Left) != 0)
        {
            var next = Math.Clamp(width - dx, minWidth, Math.Max(minWidth, left + width));
            left += width - next;
            width = next;
        }
        if ((edges & ResizeEdges.Right) != 0)
            width = Math.Clamp(width + dx, minWidth, Math.Max(minWidth, workspaceWidth - left));
        if ((edges & ResizeEdges.Top) != 0)
        {
            var next = Math.Clamp(height - dy, minHeight, Math.Max(minHeight, top + height));
            top += height - next;
            height = next;
        }
        if ((edges & ResizeEdges.Bottom) != 0)
            height = Math.Clamp(height + dy, minHeight, Math.Max(minHeight, workspaceHeight - top));

        return new WindowBounds(Math.Max(0, left), Math.Max(0, top), width, height);
    }

    public static WindowBounds Constrain(WindowBounds bounds, double workspaceWidth, double workspaceHeight,
        double minimumVisible = 80)
    {
        return bounds with
        {
            Left = Math.Clamp(bounds.Left, 0, Math.Max(0, workspaceWidth - minimumVisible)),
            Top = Math.Clamp(bounds.Top, 0, Math.Max(0, workspaceHeight - minimumVisible))
        };
    }
}
