using Spectre.Console;
namespace AnsiKit.Core;

/// <summary>
///     Opinionated wrappers for Spectre.Console prompts.
/// </summary>
public static class Prompts {
    /// <summary>Single selection list with optional search and page size.</summary>
    public static T Select<T>(string title, IEnumerable<T> choices, bool enableSearch = true, int? pageSize = null) where T : notnull {
        SelectionPrompt<T> prompt = new SelectionPrompt<T>()
            .Title( $"[cyan]{Markup.Escape( title )}[/]" )
            .MoreChoicesText( "[grey](Move up and down to reveal more)[/]" )
            .HighlightStyle( new Style( Color.Aqua ) );

        if ( enableSearch ) {
            prompt.EnableSearch(); // Available on SelectionPrompt
        }

        if ( pageSize is { } ps and > 0 ) {
            prompt.PageSize( ps );
        }

        prompt.AddChoices( choices );
        return AnsiConsole.Prompt( prompt );
    }

    /// <summary>Multi selection list with optional minimum/maximum selections.</summary>
    public static IReadOnlyList<T> MultiSelect<T>(string title, IEnumerable<T> choices, int? min = null, int? max = null) where T : notnull {
        // Note: MultiSelectionPrompt in Spectre.Console does not support search/min/max built-ins.
        // We emulate min/max after prompting.
        MultiSelectionPrompt<T> prompt = new MultiSelectionPrompt<T>()
            .Title( $"[cyan]{Markup.Escape( title )}[/]" )
            .InstructionsText( "[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to accept)[/]" )
            .HighlightStyle( new Style( Color.Aqua ) );

        prompt.AddChoices( choices );

        while (true) {
            List<T> result = AnsiConsole.Prompt( prompt );

            if ( min is { } mi && result.Count < mi ) {
                AnsiConsole.MarkupLine( $"[red]Select at least {mi} item(s).[/]" );
                continue;
            }

            if ( max is { } ma && result.Count > ma ) {
                AnsiConsole.MarkupLine( $"[red]Select at most {ma} item(s).[/]" );
                continue;
            }

            return result;
        }
    }

    /// <summary>Ask for text with optional default value and validator.</summary>
    public static string Text(string prompt, string? defaultValue = null, Func<string, ValidationResult>? validate = null, bool allowEmpty = false) {
        TextPrompt<string> tp = new($"[cyan]{Markup.Escape( prompt )}[/]") {
            // AllowEmpty is a property (not a method)
            AllowEmpty = allowEmpty,
        };

        if ( !string.IsNullOrEmpty( defaultValue ) ) {
            tp.DefaultValue( defaultValue! );
        }

        if ( validate is not null ) {
            // Prefer properties to avoid generic inference issues
            tp.Validator = validate;
            tp.ValidationErrorMessage = "[red]Invalid input[/]";
        }

        return AnsiConsole.Prompt( tp );
    }

    /// <summary>Ask for a secret (password) with optional mask character.</summary>
    public static string Secret(string prompt, char? mask = '*') {
        TextPrompt<string> tp = new($"[cyan]{Markup.Escape( prompt )}[/]");

        if ( mask is { } m ) {
            tp.Secret( m ); // extension overload with mask
        }
        else {
            tp.Secret(); // extension overload without mask
        }

        return AnsiConsole.Prompt( tp );
    }

    /// <summary>Ask for confirmation with default value.</summary>
    public static bool Confirm(string prompt, bool defaultValue = false)
        => AnsiConsole.Confirm( $"[cyan]{Markup.Escape( prompt )}[/]", defaultValue );
}