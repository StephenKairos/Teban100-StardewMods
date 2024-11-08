using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Teban100.Common;
using SObject = StardewValley.Object;

namespace AutoGate;

/// <summary>The mod entry class.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Fields
    *********/
    /// <summary>The non-gate fences in the current location.</summary>
    private readonly PerScreen<Dictionary<Vector2, Fence>> Fences = new(() => new());

    /// <summary>The gates in the current location.</summary>
    private readonly PerScreen<Dictionary<Vector2, Fence>> Gates = new(() => new());

    /// <summary>The last player tile position for which we checked for gates that need to be opened or closed.</summary>
    private readonly PerScreen<Point> LastPlayerTile = new(() => new Point(-1));


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        CommonHelper.RemoveObsoleteFiles(this, "AutoGate.pdb");

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
        helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }


    /*********
    ** Private methods
    *********/
    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded" />
    private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
    {
        this.ResetFenceCache();
    }

    /// <inheritdoc cref="IPlayerEvents.Warped" />
    private void OnWarped(object sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
            this.ResetFenceCache();
    }

    /// <inheritdoc cref="IWorldEvents.ObjectListChanged" />
    private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
    {
        this.ResetFenceCache();
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked" />
    private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
    {
        Dictionary<Vector2, Fence> fences = this.Fences.Value;

        // skip if nothing to do
        if (!Context.IsWorldReady || !fences.Any())
            return;

        // Placing a gate on top of a fence doesn't raise ObjectListChanged, so recheck fences to detect gate
        // changes. We don't need to check if a gate-on-fence was removed, since the gate + fence break together.
        foreach (Fence fence in fences.Values)
        {
            if (fence.isGate.Value)
            {
                this.ResetFenceCache();
                return;
            }
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.UpdateTicked" />
    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        Dictionary<Vector2, Fence> gates = this.Gates.Value;

        // skip if nothing to do
        if (!Context.IsWorldReady || !gates.Any())
            return;

        // skip if we already handled gates from this tile
        Point playerTile = Game1.player.TilePoint;
        if (playerTile == this.LastPlayerTile.Value)
            return;
        this.LastPlayerTile.Value = playerTile;

        // step 1: get gates that should be open
        // (We need to do this before applying changes, so we don't close a double-gate when one side is out of range.)
        HashSet<Vector2> shouldBeOpen = new();
        foreach (Vector2 tile in this.GetSearchTiles(playerTile))
        {
            if (!gates.ContainsKey(tile))
                continue;

            shouldBeOpen.Add(tile);
            foreach (Vector2 connectedTile in Utility.getAdjacentTileLocations(tile))
            {
                if (gates.ContainsKey(connectedTile))
                    shouldBeOpen.Add(connectedTile);
            }
        }

        // step 2: update gates
        foreach ((Vector2 tile, Fence gate) in gates)
        {
            bool open = shouldBeOpen.Contains(tile);
            int expectedPosition = open
                ? Fence.gateOpenedPosition
                : Fence.gateClosedPosition;

            if (gate.gatePosition.Value != expectedPosition)
                gate.toggleGate(Game1.player, open: open);
        }
    }

    /// <summary>Reset the gate cache for the current location.</summary>
    private void ResetFenceCache()
    {
        Dictionary<Vector2, Fence> fences = this.Fences.Value;
        Dictionary<Vector2, Fence> gates = this.Gates.Value;

        fences.Clear();
        gates.Clear();
        this.LastPlayerTile.Value = new Point(-1);

        if (Game1.currentLocation?.objects != null)
        {
            foreach ((Vector2 tile, SObject obj) in Game1.currentLocation.objects.Pairs)
            {
                if (obj is Fence fence)
                {
                    if (fence.isGate.Value)
                        gates[tile] = fence;
                    else
                        fences[tile] = fence;
                }
            }
        }
    }

    /// <summary>Get all tiles to search for a gate.</summary>
    /// <param name="tile">The tile from which to search for gates.</param>
    private IEnumerable<Vector2> GetSearchTiles(Point tile)
    {
        Vector2 vector = new(tile.X, tile.Y);

        return Utility
            .getAdjacentTileLocationsArray(vector)
            .Concat(new[] { vector });
    }
}
