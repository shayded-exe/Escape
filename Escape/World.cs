using System;
using System.Collections.Generic;

namespace Escape
{
	static class World
	{
		#region Declarations
		public static List<Item> Items = new List<Item>();
		public static List<Enemy> Enemies = new List<Enemy>();
		public static List<Location> Map = new List<Location>();
		#endregion
		
		#region Initialization
		public static void Initialize()
		{
			GenerateWorld();
			GenerateItems();
			GenerateEnemies();
			GenerateAttacks();
		}
		#endregion
		
		#region Generate World
		private static void GenerateWorld()
		{
			Map.Add(new Location(
				"Room 1",
				"This is a room.",
				new List<string>() { "Room 2" },
				new List<string>() { "Brass Key", "Rock" }));
				
			Map.Add(new Location(
				"Room 2",
				"This is another room.",
				new List<string>() { "Room 1", "Room 3" },
				new List<string>() { "Shiny Stone" },
				new List<string>() { "Rat" },
				50));
				
			Map.Add(new Location(
				"Room 3",
				"This is yet another room.",
				new List<string>() { "Room 2" },
				new List<string>(),
				new List<string>() { "Rat", "Hawk" },
				75));
			
			Map.Add(new Location(
				"Secret Room",
				"This is a very awesome secret room.",
				new List<string>() { "Room 3" }));
		}
		#endregion

		#region Generate Items
		private static void GenerateItems()
		{
			Items.Add(new Key(
				"Brass Key",
				"Just your generic key that's in almost every game.",
				"Room 3", "Secret Room",
				true));
				
			Items.Add(new ShinyStone(
				"Shiny Stone",
				"Its a stone, and its shiny, what more could you ask for?"));
				
			Items.Add(new Rock(
				"Rock",
				"It doesn't do anything, however, it is said that the mystical game designer used this for testing."));
		}
		#endregion

		#region Generate Enemies
		private static void GenerateEnemies()
		{
			Enemies.Add(new Rat(
				"Rat",
				"Its just a pwesious wittle wat that will KILL YOU!",
				new List<int>() { 10, 3, 5 },
				new List<string>() { }));

			Enemies.Add(new Hawk(
				"Hawk",
				"It flies around looking for prey to feed on.",
				new List<int>() { 15, 5, 0 },
				new List<string>() { }));
		}
		#endregion

		#region Generate Attacks
		private static void GenerateAttacks()
		{

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

		public static void SetExits(string room, List<string> exits)
		{
			Map[GetLocationIdByName(room)].SetExits(exits);
		}

		public static void LocationHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);
			
			List<string> locationDesctiption = Text.Limit(Map[GetLocationIdByName(Player.Location)].Description, Console.WindowWidth - 4);
			
			foreach (string line in locationDesctiption)
			{
				Text.WriteColor("| `w`" + line + Text.BlankSpaces((Console.WindowWidth - line.Length - 4), true) + "`c` |", false);
			}
			
			Text.WriteColor(">-----------------v-----------------v-----------------v-----------------<", false);
			Text.WriteColor("|      `w`Exits`c`      |      `w`Items`c`      |     `w`People`c`      |      `w`Stats`c`      |", false);
			Text.WriteColor(">-----------------#-----------------#-----------------#-----------------<`w`", false);
			
			int currentY = Console.CursorTop;
			int i;
			int longestList = 0;
			
			for (i = 0; i < Map[GetLocationIdByName(Player.Location)].Exits.Count; i++)
			{
				string name = Map[GetLocationIdByName(Player.Location)].Exits[i];
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			
			Console.SetCursorPosition(18, currentY);
			
			for (i = 0; i < Map[GetLocationIdByName(Player.Location)].Items.Count; i++)
			{
				string name = Map[GetLocationIdByName(Player.Location)].Items[i];
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			
			Console.SetCursorPosition(36, currentY);
			
			for (i = 0; i < Map[GetLocationIdByName(Player.Location)].Enemies.Count; i++)
			{
				string name = Map[GetLocationIdByName(Player.Location)].Enemies[i];
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			
			Console.SetCursorPosition(54, currentY);
			
			Text.WriteColor("  HP [`r`" + Text.ToBar(Player.Health, Player.MaxHealth, 10) + "`w`]");
			Text.WriteColor("  MP [`g`" + Text.ToBar(Player.Magic, Player.MaxMagic, 10) + "`w`]");
			
			longestList = (2 > longestList) ? 2 : longestList;
						
			Console.SetCursorPosition(0, currentY);
			
			for (i = 0; i < longestList; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Text.WriteColor("`c`|", false);
					Console.CursorLeft += 17;
				}
				
				Text.Write("|");
				Console.CursorLeft = 0;
			}
			
			Text.WriteColor("\\-----------------^-----------------^-----------------^-----------------/`w`");
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
		
		public static void ItemDescription(string aItem)
		{
			Text.WriteLine(Items[GetItemIdByName(aItem)].Description);
			Text.BlankLines();
		}
		#endregion

		#region Public Enemy Methods
		public static int GetEnemyIdByName(string enemyName)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Enemies[i].Name.ToLower() == enemyName.ToLower())
					return i;
			}

			return -1;
		}
		#endregion
	}
}