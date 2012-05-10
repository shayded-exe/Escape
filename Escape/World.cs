using System;
using System.Collections.Generic;

namespace Escape
{
	static class World
	{
		#region Declarations
		public static List<Location> Map = new List<Location>();
		public static List<Item> Items = new List<Item>();
		
		private static int locationDescriptionX;
		private static int locationDescriptionY;
		private static int locationItemsX;
		private static int locationItemsY;
		#endregion
		
		#region Initialization
		public static void Initialize()
		{
			GenerateWorld();
			GenerateItems();
		}
		#endregion
		
		#region World Generation Methods
		private static void GenerateWorld()
		{
			Map.Add(new Location(
				"Room 1",
				"This is a room.",
				new List<int>() {1},
				new List<int>() {0, 2}));
				
			Map.Add(new Location(
				"Room 2",
				"This is another room.",
				new List<int>() {0, 2},
				new List<int>() {1}));
				
			Map.Add(new Location(
				"Room 3",
				"This is yet another room.",
				new List<int>() {1}));
			
			Map.Add(new Location(
				"Secret Room",
				"This is a very awesome secret room.",
				new List<int>() {2}));
		}
		
		private static void GenerateItems()
		{
			Items.Add(new Item(
				"Brass Key",
				"Just your generic key thats in almost every game.",
				true));
				
			Items.Add(new Item(
				"Shiny Stone",
				"Its a stone, and its shiny, what more could you ask for?"));
				
			Items.Add(new Item(
				"Rock",
				"It doesn't do anything, however, it is said that the mystical game designer used this for testing."));
		}
		#endregion
		
		#region Public Location Methods
		public static bool IsLocation(string locationName)
		{
			for (int i = 0; i < Map.Count; i++)
			{
				if (Map[i].Name.ToLower() == locationName.ToLower())
					return true;
			}
			
			return false;
		}
		
		public static int GetLocationIdByName(string locationName)
		{
			for (int i = 0; i < Map.Count; i++)
			{
				if (Map[i].Name.ToLower() == locationName.ToLower())
					return i;
			}
			
			return -1;
		}
		
		public static void LocationDescription()
		{
			Text.WriteLine(Map[Player.Location].Description);
			Text.BlankLines();
		}
		
		public static void LocationExits()
		{
			if (Map[Player.Location].Exits.Count <= 0)
				return;
				
			locationDescriptionX = Console.CursorLeft + 17;
			locationDescriptionY = Console.CursorTop;
			
			Text.WriteColor("`c`/---------------\\");
			Text.WriteColor("|`w`Available Exits`c`|");
			Text.WriteLine(">---------------<");
			
			for (int i = 0; i < Map[Player.Location].Exits.Count; i++)
			{
				string name = Map[Map[Player.Location].Exits[i]].Name;
				Text.WriteColor("|`w`" + name + Text.BlankSpaces(15 - name.Length, true) + "`c`|");
			}
			
			Text.WriteColor("\\---------------/`w`");
			Text.BlankLines();
		}
		
		public static void LocationItems()
		{
			if (Map[Player.Location].Items.Count <= 0)
				return;
				
			locationItemsX = Console.CursorLeft + 17;
			locationItemsY = Console.CursorTop;
			
			Console.SetCursorPosition(locationDescriptionX + 2, locationDescriptionY);
			
			Text.WriteColor("`g`/---------------\\");
			Text.WriteColor("|`w`     Items     `g`|");
			Text.WriteLine(">---------------<");
			
			for (int i = 0; i < Map[Player.Location].Items.Count; i++)
			{
				string name = Items[Map[Player.Location].Items[i]].Name;
				Text.WriteColor("|`w`" + name + Text.BlankSpaces(15 - name.Length, true) + "`g`|");
			}
			
			Text.WriteColor("\\---------------/`w`");
			Text.BlankLines();
			
			Console.CursorLeft = 0;
		}
		#endregion
		
		#region Public Item Methods
		public static bool IsItem(string itemName)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Name.ToLower() == itemName.ToLower())
					return true;
			}
			
			return false;
		}
		
		public static int GetItemIdByName(string itemName)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Name.ToLower() == itemName.ToLower())
					return i;
			}
			
			return -1;
		}
		
		public static void ItemDescription(int itemId)
		{
			Text.WriteLine(Items[itemId].Description);
			Text.BlankLines();
		}
		#endregion
	}
}