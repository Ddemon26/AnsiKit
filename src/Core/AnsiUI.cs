using Spectre.Console;
namespace AnsiKit.Core;

/// <summary>
///     Tiny helpers over Spectre.Console for consistent, styled CLI output.
/// </summary>
public static class AnsiUi {
    public static void Header(string text, Color? color = null) {
        AnsiConsole.Write(
            new FigletText( text )
                .Centered()
                .Color( color ?? Color.Blue )
        );
    }

    /// <summary>Displays a success message in green.</summary>
    public static void Ok(string message) =>
        AnsiConsole.MarkupLine( $"[green]{Markup.Escape( message )}[/]" );

    /// <summary>Displays an informational message in cyan.</summary>
    public static void Info(string message) =>
        AnsiConsole.MarkupLine( $"[cyan]{Markup.Escape( message )}[/]" );

    /// <summary>Displays a warning message in yellow.</summary>
    public static void Warn(string message) =>
        AnsiConsole.MarkupLine( $"[yellow]{Markup.Escape( message )}[/]" );

    /// <summary>Displays an error message with a title in red.</summary>
    public static void Error(string title, string message) =>
        AnsiConsole.MarkupLine( $"[red]{Markup.Escape( title )}: {Markup.Escape( message )}[/]" );

    /// <summary>Writes a formatted exception to the console (defaults to ShortenEverything).</summary>
    public static void Exception(Exception ex, ExceptionFormats formats = ExceptionFormats.ShortenEverything) =>
        AnsiConsole.WriteException( ex, formats );

    /// <summary>Displays a note message in gray.</summary>
    public static void Note(string message) =>
        AnsiConsole.MarkupLine( $"[gray]{Markup.Escape( message )}[/]" );

    /// <summary>Horizontal rule with optional title/color/alignment.</summary>
    public static void Rule(string? title = null, Color? color = null, Justify justify = Justify.Left) {
        var rule = new Rule( title is null ? string.Empty : $"[bold]{Markup.Escape( title )}[/]" ) {
            Justification = justify,
        };
        if ( color is { } c ) {
            rule.Style = new Style( c );
        }

        AnsiConsole.Write( rule );
    }

    /// <summary>Prompts the user for input with a styled question.</summary>
    public static string Ask(string prompt)
        => AnsiConsole.Ask<string>( $"[cyan]{Markup.Escape( prompt )}[/]" );

    /// <summary>Prompts the user for confirmation with a styled question.</summary>
    public static bool Confirm(string prompt, bool defaultValue = false)
        => AnsiConsole.Confirm( $"[cyan]{Markup.Escape( prompt )}[/]", defaultValue );

    /// <summary>
    ///     Displays a status message with a spinner while executing an asynchronous task.
    /// </summary>
    public static Task StatusAsync(
        string text,
        Func<Task> body,
        Spinner? spinner = null
    )
        => AnsiConsole.Status()
            .Spinner( spinner ?? Spinner.Known.Star )
            .StartAsync( Markup.Escape( text ), async _ => { await body().ConfigureAwait( false ); } );

    /// <summary>Determines if the given input matches the specified command, ignoring case.</summary>
    public static bool IsCommand(string input, string command) =>
        string.Equals( input, command, StringComparison.OrdinalIgnoreCase );

    /// <summary>Joins a collection of strings into a single comma-separated string, escaping each value.</summary>
    public static string JoinCommaEscaped(IEnumerable<string> values) =>
        string.Join( ", ", values.Select( Markup.Escape ) );

    /// <summary>Prints the specified number of blank lines to the console.</summary>
    public static void Line(int count = 1) {
        for (var i = 0; i < count; i++) AnsiConsole.WriteLine();
    }

    /// <summary>Clears the console screen.</summary>
    public static void Clear() => AnsiConsole.Clear();

    /// <summary>
    ///     Prompts the user for input with optional validation and the ability to allow empty responses.
    /// </summary>
    public static string Prompt(string prompt, bool allowEmpty = false, Func<string, ValidationResult>? validate = null) {
        TextPrompt<string> tp = new($"[cyan]{Markup.Escape( prompt )}[/]");
        if ( allowEmpty ) {
            tp.AllowEmpty();
        }

        if ( validate != null ) {
            tp = tp.Validate( validate )
                .ValidationErrorMessage( "[red]Invalid input[/]" );
        }

        return AnsiConsole.Prompt( tp );
    }

    /// <summary>Displays a REPL-style prompt with a label and optional empty input allowance.</summary>
    public static string Repl(string label, bool allowEmpty = true) {
        TextPrompt<string> tp = new($"[blue]{Markup.Escape( label )}[/] [grey]>[/]");
        if ( allowEmpty ) {
            tp.AllowEmpty();
        }

        return AnsiConsole.Prompt( tp );
    }

    /// <summary>
    ///     Ensures the application is running in an interactive terminal. Displays guidance if not.
    /// </summary>
    public static bool RequireInteractiveTerminal(string? example = null) {
        var prof = AnsiConsole.Profile;
        var caps = prof.Capabilities;
        if ( caps.Interactive && prof.Out.IsTerminal ) return true;

        AnsiConsole.MarkupLine( "[red]❌ Interactive mode requires a terminal. Use specific commands instead.[/]" );
        if ( !string.IsNullOrWhiteSpace( example ) ) {
            AnsiConsole.MarkupLine( $"[yellow]Example: {Markup.Escape( example! )}[/]" );
        }

        return false;

    }
}