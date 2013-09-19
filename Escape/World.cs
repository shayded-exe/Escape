using System;
using System.Collections.Generic;

namespace Escape
{
	static class World
	{
		#region Declarations
		public static List<Location> Map = new List<Location>();
		public static List<Item> Items = new List<Item>();
		public static List<Enemy> Enemies = new List<Enemy>();
		public static List<Attack> Attacks = new List<Attack>();
		#endregion
		
		#region Initialization
		public static void Initialize()
		{
			GenerateWorld();
			GenerateItems();
			GenerateEnemies();
			ConvertAttributeListsToIDs();
		}
		#endregion
		
		#region World Generation Methods
		private static void GenerateWorld()
		{
			Map.Add(new Location(
				"Room 1",
				"This is a room.",
				new List<string>() { "room 2" },
				new List<string>() { "brass key", "rock" }));
				
			Map.Add(new Location(
				"Room 2",
				"This is another room.",
				new List<string>() { "room 1", "room 3" },
				new List<string>() { "shiny stone" },
				new List<string>() { "rat" },
				50));
				
			Map.Add(new Location(
				"Room 3",
				"This is yet another room.",
				new List<string>() { "room 2" },
				new List<string>(),
				new List<string>() { "rat", "hawk" },
				75));
			
			Map.Add(new Location(
				"Secret Room",
				"This is a very awesome secret room.",
				new List<string>() { "room 3" }));
		}
		
		private static void GenerateItems()
		{
			Items.Add(new Key(
				"Brass Key",
				"Just your generic key that's in almost every game.",
				"room 3", "secret room",
				true));
				
			Items.Add(new ShinyStone(
				"Shiny Stone",
				"Its a stone, and its shiny, what more could you ask for?"));
				
			Items.Add(new Rock(
				"Rock",
				"It doesn't do anything, however, it is said that the mystical game designer used this for testing."));
		}
		
		private static void GenerateEnemies()
		{
			Enemies.Add(new Enemy(
				"Rat",
				"Its just a pwesious wittle wat that will KILL YOU!",
				new List<int>() { 10, 5, 5, 5 },
				new List<string>() { "scratch" }));

			Enemies.Add(new Enemy(
				"Hawk",
				"It flies around looking for prey to feed on.",
				new List<int>() { 15, 0, 10, 0 },
				new List<string>() { "scratch" }));
		}

		private static void GenerateAttacks()
		{
			Attacks.Add(new Attack(
				"Flail",
				"Flail your arms like a fish out of water and hope something happens",
				new List<int>() { 5, 70, 0 },
				Attack.AttackTypes.Physical));

			Attacks.Add(new Attack(
				"Scratch",
				"The Attacker digs it's claws into the skin of it's prey. Not really as painful as it sounds.",
				new List<int>() { 10, 70, 5 },
				Attack.AttackTypes.Physical));
		}

		private static void ConvertAttributeListsToIDs()
		{
			for (int i = 0; i < Map.Count; i++)
			{
				Map[i].ConvertAttributeListsToIDs();
			}

			for (int i = 0; i < Enemies.Count; i++)
			{
				Enemies[i].ConvertAttributeListsToIDs();
			}
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
		
		public static int GetLocationIDByName(string locationName)
		{
			for (int i = 0; i < Map.Count; i++)
			{
				if (Map[i].Name.ToLower() == locationName.ToLower())
					return i;
			}
			
			return -1;
		}

		public static void LocationHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);
			
			List<string> locationDesctiption = Text.Limit(Map[Player.Location].Description, Console.WindowWidth - 4);
			
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
			
			for (i = 0; i < Map[Player.Location].Exits.Count; i++)
			{
				string name = Map[Map[Player.Location].Exits[i]].Name;
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			
			Console.SetCursorPosition(18, currentY);
			
			for (i = 0; i < Map[Player.Location].Items.Count; i++)
			{
				string name = Items[Map[Player.Location].Items[i]].Name;
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			
			Console.SetCursorPosition(36, currentY);
			
			for (i = 0; i < Map[Player.Location].Enemies.Count; i++)
			{
				string name = Enemies[Map[Player.Location].Enemies[i]].Name;
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
		
		public static int GetItemIDByName(string itemName)
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

		#region Public Enemy Methods
		public static int GetEnemyIDByName(string enemyName)
		{
			for (int i = 0; i < Enemies.Count; i++)
			{
				if (Enemies[i].Name.ToLower() == enemyName.ToLower())
					return i;
			}

			return -1;
		}
		#endregion

		#region Public Attack Methods
		public static int GetAttackIDByName(string attackName)
		{
			for (int i = 0; i < Attacks.Count; i++)
			{
				if (Attacks[i].Name.ToLower() == attackName.ToLower())
					return i;
			}

			return -1;
		}
		#endregion
	}
}