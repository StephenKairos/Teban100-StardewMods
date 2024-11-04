using System.Collections.Generic;
using System.Linq;
using InventoryCycle.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InventoryCycle;

/// <summary>The mod entry point.</summary>
internal class ModEntry : Mod
{
    /*********
    ** Fields
    *********/
    private SButton FrontKey = SButton.E;
    private SButton BackKey = SButton.Q;


    /*********
    ** Public methods
    *********/
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;

        ModConfig config = helper.ReadConfig<ModConfig>();

        this.FrontKey = config.FrontCycleKey;
        this.BackKey = config.BackCycleKey;

        this.Monitor.Log($"Loaded Cycle Keys as: \n For cycling forward: {this.FrontKey} \n For cycling backward {this.BackKey}");
    }


    /*********
    ** Private methods
    *********/
    /// <inheritdoc cref="IInputEvents.ButtonPressed" />
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button == this.FrontKey)
        {
            Item[] oldInventory = Game1.player.Items.ToArray();
            List<Item> newInventory = new List<Item>();
            for (int i = 12; i < oldInventory.Length; i++)
            {
                newInventory.Add(oldInventory[i]);
            }
            for (int i = 0; i < 12; i++)
            {
                newInventory.Add(oldInventory[i]);
            }

            Game1.player.setInventory(newInventory);
            if (Game1.activeClickableMenu is GameMenu)
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }
        else if (e.Button == this.BackKey)
        {
            Item[] oldInventory = Game1.player.Items.ToArray();
            List<Item> newInventory = new List<Item>();
            for (int i = 24; i < oldInventory.Length; i++)
            {
                newInventory.Add(oldInventory[i]);
            }
            for (int i = 0; i < 24; i++)
            {
                newInventory.Add(oldInventory[i]);
            }

            Game1.player.setInventory(newInventory);
            if (Game1.activeClickableMenu is GameMenu)
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }
    }
}
