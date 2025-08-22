using Spectre.Console;
namespace AnsiKit.Async;

/// <summary>
///     Provides utility methods for safely executing asynchronous actions and handling errors.
///     Uses Spectre.Console to render failures in a readable way.
/// </summary>
public static class Safe {
    /// <summary>
    ///     Runs an asynchronous action, prints a red error message on failure, and returns an exit code.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="failureTitle">The title of the error message to display on failure. Defaults to "Fatal error".</param>
    /// <param name="writeException">If true, also writes the exception using AnsiConsole.WriteException for richer output.</param>
    /// <returns>An integer representing the exit code: 0 for success, 1 for failure.</returns>
    public static async Task<int> RunAsync(Func<Task> action, string? failureTitle = "Fatal error", bool writeException = true) {
        try {
            await action().ConfigureAwait( false );
            return 0;
        }
        catch (Exception ex) {
            string title = failureTitle ?? "Fatal error";
            AnsiConsole.MarkupLine( $"[red]{Markup.Escape( title )}: {Markup.Escape( ex.Message )}[/]" );

            if ( writeException ) {
                // Render a readable exception (stack trace, inner exceptions, etc.).
                AnsiConsole.WriteException( ex, ExceptionFormats.ShortenEverything );
            }

            return 1;
        }
    }

    /// <summary>
    ///     Executes an asynchronous function that returns a value, prints an error message on failure, and returns the default
    ///     value of the type on error.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the asynchronous function.</typeparam>
    /// <param name="action">The asynchronous function to execute.</param>
    /// <param name="failureTitle">The title of the error message to display on failure. Defaults to "Error".</param>
    /// <param name="writeException">If true, also writes the exception using AnsiConsole.WriteException for richer output.</param>
    /// <returns>The result of the asynchronous function if successful; otherwise, the default value of the type.</returns>
    public static async Task<T?> TryAsync<T>(Func<Task<T>> action, string? failureTitle = "Error", bool writeException = true) {
        try {
            return await action().ConfigureAwait( false );
        }
        catch (Exception ex) {
            string title = failureTitle ?? "Error";
            AnsiConsole.MarkupLine( $"[red]{Markup.Escape( title )}: {Markup.Escape( ex.Message )}[/]" );

            if ( writeException ) {
                AnsiConsole.WriteException( ex, ExceptionFormats.ShortenEverything );
            }

            return default;
        }
    }
}