using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;

namespace AutoGate
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The gates in the current location.</summary>
        private readonly Dictionary<Vector2, SObject> Gates = new SerializableDictionary<Vector2, SObject>();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                this.ResetGateList();
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            this.ResetGateList();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !this.Gates.Any())
                return;

            var adjacent = new HashSet<Vector2>(Utility.getAdjacentTileLocations(Game1.player.getTileLocation()));
            foreach (var pair in this.Gates)
            {
                Vector2 tile = pair.Key;
                SObject gate = pair.Value;

                if (gate.isPassable() != adjacentTiles.Contains(tile))
                    gate.checkForAction(Game1.player);
            }
        }

        /// <summary>Reset the gate cache for the current location.</summary>
        private void ResetGateList()
        {
            this.Gates.Clear();

            foreach (var pair in Game1.currentLocation.objects.Pairs)
            {
                Vector2 tile = pair.Key;
                SObject obj = pair.Value;

                if (obj.Name == "Gate")
                    this.Gates[tile] = obj;
            }
        }
    }
}
