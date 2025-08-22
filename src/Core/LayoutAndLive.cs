using Spectre.Console;
using Spectre.Console.Rendering;
namespace AnsiKit.Core;

/// <summary>Wrappers for Layout and Live display.</summary>
public static class LayoutAndLive {
    /// <summary>Create a basic 2x2 layout and render content into quadrants.</summary>
    public static Layout TwoByTwo(IRenderable? topLeft = null, IRenderable? topRight = null, IRenderable? bottomLeft = null, IRenderable? bottomRight = null) {
        var layout = new Layout( "Root" )
            .SplitColumns(
                new Layout( "Left" ),
                new Layout( "Right" ).SplitRows(
                    new Layout( "Top" ),
                    new Layout( "Bottom" )
                )
            );

        if ( topLeft != null ) {
            layout["Left"].Update( topLeft );
        }

        if ( topRight != null ) {
            layout["Right"]["Top"].Update( topRight );
        }

        if ( bottomRight != null ) {
            layout["Right"]["Bottom"].Update( bottomRight );
        }

        if ( bottomLeft != null ) {
            layout["Left"].SplitRows( new Layout( "LTop" ), new Layout( "LBottom" ) );
        }

        return layout;
    }

    /// <summary>Run a live display for dynamic updates.</summary>
    public static Task LiveAsync(IRenderable renderable, Func<LiveDisplayContext, Task> body)
        => AnsiConsole.Live( renderable ).StartAsync( body );
}