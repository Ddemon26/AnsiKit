using Spectre.Console;
namespace AnsiKit.Widgets;

public static class Widgets {
    /// <summary>Create and render a styled table.</summary>
    public static void Table(
        string? title,
        IEnumerable<string> headers,
        IEnumerable<IEnumerable<string>> rows,
        TableBorder? border = null
    ) {
        var table = new Table();

        if ( !string.IsNullOrWhiteSpace( title ) ) {
            table.Title = new TableTitle( $"[bold]{Markup.Escape( title! )}[/]" );
        }

        foreach (string header in headers) {
            table.AddColumn( $"[yellow]{Markup.Escape( header )}[/]" );
        }

        foreach (IEnumerable<string> row in rows) {
            table.AddRow( row.Select( Markup.Escape ).ToArray() );
        }

        table.Border = border ?? TableBorder.Rounded;
        table.Expand();
        AnsiConsole.Write( table );
    }

    /// <summary>Display multiple items in responsive columns.</summary>
    public static void Columns(IEnumerable<string> items) {
        var cols = new Columns( items.Select( i => new Markup( Markup.Escape( i ) ) ) ) {
            Expand = true,
        };
        AnsiConsole.Write( cols );
    }

    /// <summary>Run multiple tasks under a progress UI.</summary>
    public static async Task ProgressAsync(
        Func<ProgressContext, Task> action,
        bool autoClear = true
    ) {
        await AnsiConsole.Progress()
            .AutoClear( autoClear )
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn()
            )
            .StartAsync( action )
            .ConfigureAwait( false );
    }
}