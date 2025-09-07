using Spectre.Console;
using Spectre.Console.Rendering;
namespace AnsiKit.Core;

/// <summary>Wrappers for Layout and Live display.</summary>
public static class LayoutAndLive {
    /// <summary>Create a basic 2x2 layout and render content into quadrants.</summary>
    public static Layout TwoByTwo(IRenderable? topLeft = null, IRenderable? topRight = null, IRenderable? bottomLeft = null, IRenderable? bottomRight = null) {
        // Create a symmetric 2x2 layout. Each column is split into two
        // rows (top and bottom), allowing content to be rendered in any of
        // the four quadrants.
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Left"),
                new Layout("Right")
            );

        // Split both the left and right columns into top and bottom rows.
        layout["Left"].SplitRows(new Layout("LeftTop"), new Layout("LeftBottom"));
        layout["Right"].SplitRows(new Layout("RightTop"), new Layout("RightBottom"));

        // Assign provided renderables to their respective quadrants if present.
        if (topLeft != null) {
            layout["Left"]["LeftTop"].Update(topLeft);
        }
        if (bottomLeft != null) {
            layout["Left"]["LeftBottom"].Update(bottomLeft);
        }
        if (topRight != null) {
            layout["Right"]["RightTop"].Update(topRight);
        }
        if (bottomRight != null) {
            layout["Right"]["RightBottom"].Update(bottomRight);
        }

        return layout;
    }

    /// <summary>Run a live display for dynamic updates.</summary>
    public static Task LiveAsync(IRenderable renderable, Func<LiveDisplayContext, Task> body)
        => AnsiConsole.Live( renderable ).StartAsync( body );
}