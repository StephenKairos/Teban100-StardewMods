using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System;

namespace ModEntry
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		//The Storage Map for the gate objects, Vector2 is the position of the gate, bool is if it is open (true -> isOpen, false -> isClosed)
		public SerializableDictionary<Vector2, bool> gateList = new SerializableDictionary<Vector2, bool>();

		/*********
		** Public methods
		*********/
		/// <summary>Initialise the mod.</summary>
		/// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
		public override void Entry(IModHelper helper)
		{
			LocationEvents.CurrentLocationChanged += this.EnteredNewLocation;
			LocationEvents.LocationObjectsChanged += this.CreatedOrDestroyedGate;
			GameEvents.HalfSecondTick += this.ReceiveHalfSecondTick;
		}


		/*********
		** Private methods
		*********/
		/// <summary>The method invoked when the player presses a keyboard button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void ReceiveKeyPress(object sender, EventArgsKeyPressed e)
		{
			this.Monitor.Log($"Player pressed {e.KeyPressed}.");
		} 
			{
				if (objectList[key] is StardewValley.Fence)
				{
					gateList.Add(key, false);
					if (Game1.currentLocation.objects[key].isPassable())
					{
						Game1.currentLocation.objects[key].checkForAction(Game1.player);
	}
}
			}
		}

		private void CreatedOrDestroyedGate(object sender, EventArgsLocationObjectsChanged e)
{

	gateList = new SerializableDictionary<Vector2, bool>();
	SerializableDictionary<Vector2, StardewValley.Object> objectList = Game1.currentLocation.objects;
	foreach (Vector2 key in objectList.Keys)
	{
		if (objectList[key] is StardewValley.Fence)
		{
			gateList.Add(key, false);
			if (Game1.currentLocation.objects[key].isPassable())
			{
				Game1.currentLocation.objects[key].checkForAction(Game1.player);
			}
		}
	}
}

private void ReceiveHalfSecondTick(object sender, EventArgs e)
{
	Vector2[] adjPos = Utility.getAdjacentTileLocations(Game1.player.getTileLocation()).ToArray();

	foreach (Vector2 gateKey in gateList.Keys)
	{
		bool nextToGate = false;
		foreach (Vector2 position in adjPos)
		{
			if (gateKey.Equals(position) || gateKey.Equals(Game1.player.position))
			{
				nextToGate = true;
			}
		}

		if (nextToGate && gateList[gateKey] == false)
		{
			Game1.currentLocation.objects[gateKey].checkForAction(Game1.player);
			gateList[gateKey] = true;
		}
		else if (!nextToGate && gateList[gateKey] == true)
		{
			Game1.currentLocation.objects[gateKey].checkForAction(Game1.player);
			gateList[gateKey] = false;
		}
	}
}
	}
}