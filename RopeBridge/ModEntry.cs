using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace RopeBridge
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
            helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
            helper.Events.World.NpcListChanged += this.OnNpcListChanged;
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
                this.FixLadders();
        }

        /// <summary>Raised after objects are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (e.IsCurrentLocation)
                this.FixLadders();
        }

        /// <summary>Raised after NPCs are added or removed in a location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (e.IsCurrentLocation && e.Removed.Any())
                this.FixLadders();
        }

        /// <summary>Detect any ladders in the current location and mark them passable.</summary>
        private void FixLadders()
        {
            if (Game1.currentLocation is MineShaft)
            {
                Layer layer = Game1.currentLocation.map.GetLayer("Buildings");
                if (layer == null)
                    return;

                for (int x = 0; x < layer.LayerWidth; x++)
                {
                    for (int y = 0; y < layer.LayerHeight; y++)
                    {
                        Tile tile = layer.Tiles[x, y];
                        if (tile?.TileIndex == 173)
                            tile.TileIndexProperties.Add(new KeyValuePair<string, PropertyValue>("Passable", "T"));
                    }
                }
            }
        }
    }
}
