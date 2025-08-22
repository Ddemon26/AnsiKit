using System.Globalization;
using Spectre.Console;
using Spectre.Console.Rendering;
using Calendar = Spectre.Console.Calendar;
namespace AnsiKit.Core;

/// <summary>Wrappers for Panel, Rule, Grid, Calendar.</summary>
public static class PanelsAndGrid {
    public static void Panel(string content, string? header = null, Color? borderColor = null, BoxBorder? border = null, Justify? headerJustify = null) {
        var panel = new Panel( new Markup( Markup.Escape( content ) ) );
        if ( !string.IsNullOrWhiteSpace( header ) ) {
            panel.Header = new PanelHeader( $"[bold]{Markup.Escape( header! )}[/]" );
            if ( headerJustify is { } j ) {
                panel.Header = panel.Header.Justify( j );
            }
        }

        if ( borderColor is not null ) { }

        panel.Border = border ?? BoxBorder.Rounded;
        if ( borderColor is { } c ) {
            panel.BorderStyle = new Style( c );
        }

        panel.Expand();
        AnsiConsole.Write( panel );
    }

    public static void Rule(string? title = null, Color? color = null, Justify justify = Justify.Left) {
        var rule = new Rule( title is null ? string.Empty : $"[bold]{Markup.Escape( title )}[/]" ) {
            Justification = justify,
        };
        if ( color is { } c ) {
            rule.Style = new Style( c );
        }

        AnsiConsole.Write( rule );
    }

    public static void Grid(IEnumerable<IEnumerable<string>> rows, IEnumerable<int>? columnPaddings = null) {
        var grid = new Grid();
        IEnumerable<string>[] enumerable = rows as IEnumerable<string>[] ?? rows.ToArray();
        int? columns = enumerable.FirstOrDefault()?.Count();
        if ( columns is { } colCount ) {
            for (var i = 0; i < colCount; i++) {
                grid.AddColumn();
            }
        }

        if ( columnPaddings != null ) {
            var i = 0;
            foreach (int pad in columnPaddings) {
                grid.Columns.ElementAt( i ).Padding = new Padding( pad, 0 );
                i++;
            }
        }

        foreach (IEnumerable<string> row in enumerable) {
            grid.AddRow( row.Select( s => new Markup( Markup.Escape( s ) ) ).ToArray<IRenderable>() );
        }

        grid.Expand();
        AnsiConsole.Write( grid );
    }

    public static void Calendar(DateTime date, IEnumerable<(int Year, int Month, int Day, string? Description, Style? Style)>? events = null, CultureInfo? culture = null) {
        var cal = new Calendar( date );
        if ( culture != null ) {
            cal.Culture( culture.Name );
        }

        if ( events != null ) {
            foreach (var e in events) {
                cal.AddCalendarEvent( e.Year, e.Month, e.Day, e.Style );
            }
        }

        AnsiConsole.Write( cal );
    }
}