using Spectre.Console;
namespace AnsiKit.Core;

/// <summary>Helper to emit trees quickly.</summary>
public static class AnsiTree {
    public static void Tree(string root, IEnumerable<(string Path, string? Emoji)> nodes) {
        var tree = new Tree( new Markup( Markup.Escape( root ) ) );
        Dictionary<string, TreeNode> map = new(); // full path -> node

        foreach ((string path, string? emoji) in nodes) {
            string[] parts = path.Split( '/', StringSplitOptions.RemoveEmptyEntries );
            var current = "";
            TreeNode? parent = null;

            foreach (string part in parts) {
                current = current.Length == 0 ? part : $"{current}/{part}";

                if ( !map.TryGetValue( current, out var node ) ) {
                    var label = $"{Markup.Escape( part )}{(emoji is null ? "" : " " + emoji)}";
                    node = parent is null ? tree.AddNode( label ) : parent.AddNode( label );
                    map[current] = node;
                }

                parent = map[current];
            }
        }

        // Tree implements an Expanded property (no Root or Expand() in this API)
        tree.Expanded = true;
        AnsiConsole.Write( tree );
    }
}