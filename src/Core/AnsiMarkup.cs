using System;
using Spectre.Console;

namespace AnsiKit.Core;

/// <summary>
///     Helper methods for writing markup to the console. These wrap
///     Spectre.Console's markup APIs and provide safe escaping for
///     interpolated strings.
/// </summary>
public static class AnsiMarkup {
    /// <summary>
    ///     Writes a markup-formatted string to the console. Equivalent to
    ///     <see cref="Spectre.Console.AnsiConsole.Markup(string)"/>.
    /// </summary>
    /// <param name="value">The markup-formatted string to write.</param>
    public static void Write(string value) {
        // Delegate directly to Spectre.Console. The user must supply a
        // properly formatted markup string; use <see cref="Escape"/> to
        // escape arbitrary text.
        AnsiConsole.Markup(value);
    }

    /// <summary>
    ///     Writes a markup-formatted string followed by a newline to the
    ///     console. Equivalent to
    ///     <see cref="Spectre.Console.AnsiConsole.MarkupLine(string)"/>.
    /// </summary>
    /// <param name="value">The markup-formatted string to write.</param>
    public static void WriteLine(string value) {
        AnsiConsole.MarkupLine(value);
    }

    /// <summary>
    ///     Writes an interpolated string as markup to the console. Any
    ///     interpolated expressions are automatically escaped to prevent
    ///     markup injection. Equivalent to
    ///     <see cref="Spectre.Console.AnsiConsole.MarkupInterpolated(FormattableString)"/>.
    /// </summary>
    /// <param name="value">The interpolated string containing markup.
    /// Interpolated expressions will be escaped automatically.</param>
    public static void WriteInterpolated(FormattableString value) {
        AnsiConsole.MarkupInterpolated(value);
    }

    /// <summary>
    ///     Writes an interpolated string as markup followed by a newline to
    ///     the console. Equivalent to
    ///     <see cref="Spectre.Console.AnsiConsole.MarkupLineInterpolated(FormattableString)"/>.
    /// </summary>
    /// <param name="value">The interpolated string containing markup.
    /// Interpolated expressions will be escaped automatically.</param>
    public static void WriteLineInterpolated(FormattableString value) {
        AnsiConsole.MarkupLineInterpolated(value);
    }

    /// <summary>
    ///     Escapes a string so that any markup control characters (like '[' or
    ///     ']') are treated as literals rather than markup. This is a
    ///     thin wrapper around <see cref="Spectre.Console.Markup.Escape(string)"/>.
    /// </summary>
    /// <param name="text">The text to escape. A null value will be treated
    /// as an empty string.</param>
    /// <returns>The escaped string, safe to embed in markup.</returns>
    public static string Escape(string? text) {
        return Markup.Escape(text ?? string.Empty);
    }
}