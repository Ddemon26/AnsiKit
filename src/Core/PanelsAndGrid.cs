using System.Globalization;
using Spectre.Console;
using Spectre.Console.Rendering;
using Calendar = Spectre.Console.Calendar;
namespace AnsiKit.Core;

/// <summary>Wrappers for Panel, Rule, Grid, Calendar.</summary>
public static class PanelsAndGrid {
    public static void Panel(string content, string? header = null, Color? borderColor = null, BoxBorder? border = null, Justify? headerJustify = null) {
        // Create the panel with escaped content to ensure any brackets are literal.
        var panel = new Panel(new Markup(Markup.Escape(content)));

        // Configure the header if provided. Use bold markup to distinguish it.
        if (!string.IsNullOrWhiteSpace(header)) {
            var hdr = new PanelHeader($"[bold]{Markup.Escape(header!)}[/]");
            if (headerJustify is { } j) {
                hdr = hdr.Justify(j);
            }
            panel.Header = hdr;
        }

        // Apply border style and optional border color. If no border is given
        // default to rounded corners for consistency with other widgets.
        panel.Border = border ?? BoxBorder.Rounded;
        if (borderColor is { } c) {
            panel.BorderStyle = new Style(c);
        }

        // Expand to fill the available console width.
        panel.Expand();
        AnsiConsole.Write(panel);
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
        var cal = new Calendar(date);

        // Apply optional culture to localize month/day names.
        if (culture != null) {
            cal.Culture(culture.Name);
        }

        // Add calendar events if provided. Support descriptions and optional
        // highlight styles. When a description is provided the overload with
        // a description will be used, otherwise the simple overload is used.
        if (events != null) {
            foreach (var e in events) {
                if (!string.IsNullOrWhiteSpace(e.Description)) {
                    // Use the overload that accepts a description and optional style.
                    if (e.Style != null) {
                        cal.AddCalendarEvent(e.Description!, e.Year, e.Month, e.Day, e.Style);
                    } else {
                        cal.AddCalendarEvent(e.Description!, e.Year, e.Month, e.Day);
                    }
                } else {
                    // No description; call the overload that accepts only the date and optional style.
                    if (e.Style != null) {
                        cal.AddCalendarEvent(e.Year, e.Month, e.Day, e.Style);
                    } else {
                        cal.AddCalendarEvent(e.Year, e.Month, e.Day);
                    }
                }
            }
        }

        AnsiConsole.Write(cal);
    }
}