﻿using System;
using System.Collections.Generic;

namespace Escape
{
	static class World
	{
		#region Declarations
		//Creates empty lists to hold all the information about the world
		public static List<Location> Map = new List<Location>();
		public static List<Item> Items = new List<Item>();
		public static List<Enemy> Enemies = new List<Enemy>();
		public static List<Attack> Attacks = new List<Attack>();
		#endregion
		
		#region Initialization
		//Generates the data that goes into the above lists
		public static void Initialize()
		{
			GenerateWorld();
			GenerateItems();
			GenerateEnemies();
			GenerateAttacks();
			ConvertAttributeListsToIDs();
		}
		#endregion
		
		#region World Generation Methods
		/*
		 * This defines all the locations that exist in the map along with their specific properties
		 * 
		 * Format:
		 *     Map.Add(new Location(
		 *         "Name",
		 *         "Description",
		 *         new List<string>() { List of exits by name },
		 *         new List<string>() { List of items by name },
		 *         new List<string>() { List of enemies by name },
		 *         What the chance of a random battle in that room is ( x / 100 )));
		 */
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

		/*
		 * This defines all the items exist in the game.
		 */
		private static void GenerateItems()
		{
            // Key
            Item key = new Item("Brass Key");
            key.Description = "Just your generic key that's in almost every game.";
            key.Usable = true;
            key.ExtendedAttributes.Add("str_targetLocation", "room 3");
            key.ExtendedAttributes.Add("str_newLocation", "secret room");
            key.Uses += new Item.OnUse(delegate(Item self)
                {
                    if (Player.Location == World.GetLocationIDByName((string)self.ExtendedAttributes["str_targetLocation"]))
                    {
                        Program.SetNotification("The " + self.Name + " opened the lock!");
                        World.Map[World.GetLocationIDByName((string)self.ExtendedAttributes["str_targetLocation"])].Exits.Add(
                            World.GetLocationIDByName((string)self.ExtendedAttributes["str_newLocation"]));
                    }
                    else
                        self.NoUse();
                });
            Items.Add(key);

            // Shiny Stone
            Item shinyStone = new Item("Shiny Stone");
            shinyStone.Description = "It's a stone and it's shiny, what more could you ask for?";
            shinyStone.Usable = true;
            shinyStone.UsableInBattle = true;
            shinyStone.Uses += new Item.OnUse(delegate(Item self)
                {
                    if (Player.Location == World.GetLocationIDByName("secret room"))
                    {
                        Player.Health += Math.Min(Player.MaxHealth / 10, Player.MaxHealth - Player.Health);
                        Program.SetNotification("The magical stone restored your health by 10%!");
                    }
                    else
                        Program.SetNotification("The shiny stone glowed shiny colors!");
                });
            Items.Add(shinyStone);

            // Rock
            Item rock = new Item("Rock");
            rock.Description = "It doesn't do anything, however, it is said that the mystical game designer used this for testing.";
            rock.Usable = true;
            rock.UsableInBattle = true;
            rock.Uses += new Item.OnUse(delegate(Item self)
                {
                    Program.SetNotification("You threw the rock at a wall. Nothing happened.");
                });
            rock.BattleUses += new Item.OnUseInBattle(delegate(Item self, Enemy victim)
                {
                    Program.SetNotification("The rock hit the enemy in the head! It seems confused...");
                });
            Items.Add(rock);
		}
		
		/*
		 * This defines all the enemies that exist in the game
		 * 
		 * Format:
		 *     Map.Add(new Enemy(    *Enemy could also be substituted by a specific subclass if an enemy has one*
		 *         "Name",
		 *         "Description",
		 *         new List<int>() { Health, Magic, Power, Defense, ExpValue },
		 *         new List<string>() { List of attacks by name }));
		 */
		private static void GenerateEnemies()
		{
			Enemies.Add(new Enemy(
				"Rat",
				"Its just a pwesious wittle wat that will KILL YOU!",
				new List<int>() { 10, 5, 10, 5, 5 },
				new List<string>() { "scratch" }));

			Enemies.Add(new Enemy(
				"Hawk",
				"It flies around looking for prey to feed on.",
				new List<int>() { 15, 0, 15, 0, 8 },
				new List<string>() { "scratch" }));
		}

		//Format: (Name, Description, Stats: (Power, Accuracy, Cost), Type)
		/*
		 * This defines all the enemies that exist in the game
		 * 
		 * Format:
		 *     Map.Add(new Attack(    *Attack could also be substituted by a specific subclass if an attack has one*
		 *         "Name",
		 *         "Description",
		 *         new List<int>() { Power, Accuracy, Magic Cost },
		 *         Attack.AttackTypes.TypeOfAttack (Not fully implemented)));
		 */
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
				new List<int>() { 10, 70, 1 },
				Attack.AttackTypes.Physical));
		}
		#endregion

		#region Public Location Methods
		//Checks if the provided string is a location
		public static bool IsLocation(string locationName)
		{
			//Iterates through every location in the map and compares the names to the one provided
			for (int i = 0; i < Map.Count; i++)
			{
				if (Map[i].Name.ToLower() == locationName.ToLower())
					return true;
			}
			
			return false;
		}

		//Checks if the provided ID is a location
		public static bool IsLocation(int locationId)
		{
			//Checks if the provided ID is lower than the total locations in the map and if that location has data stored in it
			if (Map.Count > locationId && Map[locationId] != null)
				return true;

			return false;
		}
		
		//Returns the ID of a location given its name. 
		//This works the same as IsLocation, but returns the name instead of a boolean
		public static int GetLocationIDByName(string locationName)
		{
			for (int i = 0; i < Map.Count; i++)
			{
				if (Map[i].Name.ToLower() == locationName.ToLower())
					return i;
			}
			
			return -1;
		}

		//Prints the main HUD that is displayed for most of the game. Warning, this gets a little complex.
		public static void LocationHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);
			
			List<string> locationDesctiption = Text.Limit(Map[Player.Location].Name + " - " + Map[Player.Location].Description, Console.WindowWidth - 4);
			
			foreach (string line in locationDesctiption)
			{
				Text.WriteColor("| `w`" + line + Text.BlankSpaces((Console.WindowWidth - line.Length - 4), true) + "`c` |", false);
			}
			
			Text.WriteColor(">-----------------v-----------------v-----------------v-----------------<", false);
			Text.WriteColor("|      `w`Exits`c`      |      `w`Items`c`      |     `w`People`c`      |      `w`Stats`c`      |", false);
			Text.WriteColor(">-----------------#-----------------#-----------------#-----------------<`w`", false);
			
			int currentY = Console.CursorTop;
			int i = 0;
			int longestList = 0;
			
			foreach (int exit in Map[Player.Location].Exits)
			{
				string name = Map[exit].Name;
				Text.WriteColor("  " + name);
				i++;
			}
			
			longestList = (i > longestList) ? i : longestList;
			i = 0;
			
			Console.SetCursorPosition(18, currentY);
			
			foreach (int item in Map[Player.Location].Items)
			{
				string name = Items[item].Name;
				Text.WriteColor("  " + name);
				i++;
			}
			
			longestList = (i > longestList) ? i : longestList;
			i = 0;
			
			Console.SetCursorPosition(36, currentY);
			
			foreach (int enemy in Map[Player.Location].Enemies)
			{
				string name = Enemies[enemy].Name;
				Text.WriteColor("  " + name);
			}
			
			longestList = (i > longestList) ? i : longestList;
			i = 0;
			
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
			
			Text.WriteColor("\\-----------------^-----------------+-----------------^-----------------/`w`", false);
			Text.WriteColor(" `c`\\`w` Lvl.", false);

			if (Player.Level < 10)
			{
				Text.Write(" ");
			}

			Text.WriteColor(Player.Level + " [`g`" + Text.ToBar(Player.Exp, Player.GetNextLevel(), 23) + "`w`] `c`|`w` " 
			+ Player.Exp + "/" + Player.GetNextLevel() + "`c` /", false);

			int expLength = Console.CursorLeft;

			Text.WriteLine("", false);

			Text.WriteColor("  \\---------------------------------^", false);

			while (Console.CursorLeft < expLength - 2)
			{
				Text.WriteColor("-", false);
			}

			Text.WriteColor("/`w`", false);

			Text.WriteAt("`c`v`w`", expLength, Console.CursorTop - 2, true, true);

			Text.WriteLine("", false);
			Text.WriteLine("", false);
		}
		#endregion
		
		#region Public Item Methods
		//This works the same as GetLocationIDByName
		public static int GetItemIDByName(string itemName)
		{
			foreach (Item item in Items)
			{
				if (item.Name.ToLower() == itemName.ToLower())
					return Items.IndexOf(item);
			}

			return -1;
		}

		//This works the same as IsLocation
		public static bool IsItem(string itemName)
		{
			foreach (Item item in Items)
			{
				if (item.Name.ToLower() == itemName.ToLower())
					return true;
			}
			
			return false;
		}

		//Writes the description of an item (This will be revamped soon)
		public static void ItemDescription(int itemId)
		{
			Text.WriteLine(Items[itemId].Description);
			Text.BlankLines();
		}
		#endregion

		#region Public Enemy Methods
		//This works the same as GetLocationIDByName
		public static int GetEnemyIDByName(string enemyName)
		{
			foreach (Enemy enemy in Enemies)
			{
				if (enemy.Name.ToLower() == enemyName.ToLower())
					return Enemies.IndexOf(enemy);
			}

			return -1;
		}

		//This works the same as IsLocation
		public static bool IsEnemy(string enemyName)
		{
			foreach (Enemy enemy in Enemies)
			{
				if (enemy.Name.ToLower() == enemyName.ToLower())
					return true;
			}

			return false;
		}
		#endregion

		#region Public Attack Methods
		//This works the same as GetLocationIDByName
		public static int GetAttackIDByName(string attackName)
		{
			foreach (Attack attack in Attacks)
			{
				if (attack.Name.ToLower() == attackName.ToLower())
					return Attacks.IndexOf(attack);
			}

			return -1;
		}

		//This works the same as IsLocation
		public static bool IsAttack(string attackName)
		{
			foreach (Attack attack in Attacks)
			{
				if (attack.Name.ToLower() == attackName.ToLower())
					return true;
			}

			return false;
		}
		#endregion

		#region Helper Methods
		/*
		 * This converts the names listed in the attributes of all the locations and enemies to IDs.
		 * This allows those attributes to be written as names for ease of coding, but then used by the
		 * game later on. This makes everything easier to work with later on in the game since the names
		 * don't have to keep being converted to IDs for processing.
		 */
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
	}
}