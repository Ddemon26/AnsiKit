using Spectre.Console;
using Spectre.Console.Json;
namespace AnsiKit.Core;

/// <summary>Wrappers for JSON rendering.</summary>
public static class AnsiJson {
    public static void Json(string json, Style? stringStyle = null, Style? numberStyle = null, Style? booleanStyle = null, Style? nullStyle = null) {
        var jt = new JsonText( json );
        if ( stringStyle != null ) {
            jt = jt.StringStyle( stringStyle );
        }

        if ( numberStyle != null ) {
            jt = jt.NumberStyle( numberStyle );
        }

        if ( booleanStyle != null ) {
            jt = jt.BooleanStyle( booleanStyle );
        }

        if ( nullStyle != null ) {
            jt = jt.NullStyle( nullStyle );
        }

        AnsiConsole.Write( jt );
    }
}