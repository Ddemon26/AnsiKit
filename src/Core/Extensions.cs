using Spectre.Console;

namespace AnsiKit.Core;

/// <summary>
///     Extension methods for primitive types to enhance markup handling.
/// </summary>
public static class Extensions {
    /// <summary>
    ///     Escapes markup characters in a string so that it can be safely
    ///     embedded in markup-formatted output. This mirrors the
    ///     behavior of Spectre.Console.Markup.Escape(string) but is
    ///     exposed as an extension method on <see cref="string"/> for
    ///     convenience.
    /// </summary>
    /// <param name="value">The string to escape. If null, an empty string is returned.</param>
    /// <returns>The escaped string.</returns>
    public static string EscapeMarkup(this string? value) {
        return Markup.Escape(value ?? string.Empty);
    }
}