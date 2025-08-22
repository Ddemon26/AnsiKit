using Spectre.Console;
namespace AnsiKit.Core;

/// <summary>Wrappers for charts (BarChart and BreakdownChart).</summary>
public static class Charts {
    public enum LabelAlign { Left, Center, Right }

    /// <summary>Write a horizontal bar chart.</summary>
    public static void BarChart(string? title, IEnumerable<(string Label, double Value, Color? Color)> items, int? width = null, LabelAlign labelAlign = LabelAlign.Center) {
        var chart = new BarChart();
        if ( !string.IsNullOrWhiteSpace( title ) ) {
            chart = chart.Label( title! ).CenterLabel();
        }

        if ( width is { } w and > 0 ) {
            chart = chart.Width( w );
        }

        // Apply label alignment via documented extension methods.
        chart = labelAlign switch {
            LabelAlign.Left => chart.LeftAlignLabel(),
            LabelAlign.Right => chart.RightAlignLabel(),
            _ => chart.CenterLabel(),
        };

        foreach ((string label, double value, Color? color) in items) {
            chart.AddItem( label, value, color ?? Color.Grey );
        }

        AnsiConsole.Write( chart );
    }

    /// <summary>Write a breakdown chart.</summary>
    public static void BreakdownChart(string? title, IEnumerable<(string Label, double Value, Color Color)> items, int? width = null) {
        var chart = new BreakdownChart()
            .ShowPercentage(); // shows tag values as percentages

        if ( width is { } w and > 0 ) {
            chart.Width = w; // Width is a property; this avoids relying on the fluent extension
        }

        foreach ((string label, double value, var color) in items) {
            chart.AddItem( label, value, color );
        }

        if ( !string.IsNullOrWhiteSpace( title ) ) {
            var panel = new Panel( chart ) {
                Header = new PanelHeader( title!, Justify.Center ),
            };
            AnsiConsole.Write( panel );
        }
        else {
            AnsiConsole.Write( chart );
        }
    }
}