using System;
using System.Collections.Generic;

namespace Escape
{
	static class Player
	{
		#region Declarations
		public static string Name;
		public static int Location = 0;
		
		private static int baseHealth = 100;
		private static int health = baseHealth;
		private static int healthBar = 0;
		private static List<int> inventory = new List<int>();
		
		private readonly static string[] Commands = new string[] {"help", "?", "exit", "quit", "move", "go", "examine", "take", "pickup", "drop", "place", "use", "items", "inventory", "inv", "attack"};
		private readonly static string[] BattleCommands = new string[] {"attack", "flee", "escape", "use", "items", "inventory", "inv"};
		private static string[] Attacks = new string[] {/*TODO: Add attacks*/};
		#endregion
		
		#region Public Methods
		public static void Do(string aString)
		{
			if(aString == "")
				return;
			
			string verb = "";
			string noun = "";
			
			if (aString.IndexOf(" ") > 0)
			{
				string[] temp = aString.Split(new char[] {' '}, 2);
				verb = temp[0].ToLower();
				noun = temp[1].ToLower();
			}
			else
			{
				verb = aString.ToLower();
			}
			
			switch(Program.GameState)
			{
				case Program.GameStates.Playing:
					if (IsValidInput(Commands, verb, true))
					{
						switch(verb)
						{
							case "help":
							case "?":
								WriteCommands();
								break;
							case "exit":
							case "quit":
								Program.GameState = Program.GameStates.Quit;
								break;
							case "move":
							case "go":
								MoveTo(noun);
								break;
							case "examine":
								Examine(noun);
								break;
							case "take":
							case "pickup":
								Pickup(noun);
								break;
							case "drop":
							case "place":
								Place(noun);
								break;
							case "use":
								Use(noun);
								break;
							case "items":
							case "inventory":
							case "inv":
								DisplayInventory();
								break;
							case "attack":
								//attack command
								break;
						}
					}
					break;
					
				case Program.GameStates.Battle:
					if(IsValidInput(BattleCommands, verb, true))
					{
						switch(verb)
						{
							case "attack":
								//attack command
								break;
							case "flee":
							case "escape":
								//flee command
								break;
							case "use":
								//use command
								break;
							case "items":
							case "inventory":
							case "inv":
								//items command
								break;
						}
					}
					break;
			}
		}
		#endregion
		
		#region Command Methods
		private static void WriteCommands()
		{
			Text.WriteColor("`g`Available Commands:`w`");
			Text.WriteColor("help/? - Displays this list.");
			Text.WriteColor("exit/quit - Exits the game.");
			Text.WriteColor("move/go <`c`location`w`> - Moves you to the specified location.");
			Text.WriteColor("examine <`c`item`w`> - Shows info about the specified item.");
			Text.WriteColor("take/pickuip <`c`item`w`> - Puts the specified item in your inventory.");
			Text.WriteColor("drop/place <`c`item`w`> - Drops the specified item from your inventory and places it in the world.");
			Text.WriteColor("items/inventory/inv - Displays your current inventory.");
			Text.WriteColor("use <`c`item`w`> - Uses the specified item.");
			Text.BlankLines();
			
			Text.WriteColor("`r`Not Implemented Commands:`w`");
			Text.WriteColor("attack <`c`enemy`w`> - Attacks the specified enemy.");
			Text.BlankLines();
		}
		
		private static void MoveTo(string locationName)
		{
			if (World.IsLocation(locationName))
			{
				int locationId = World.GetLocationIdByName(locationName);
				
				if (World.Map[Location].ContainsExit(locationId))
				{
					Location = locationId;
				}
				else
				{
					Program.SetError("You can't get there from here!");
				}
			}
			else
			{
				Program.SetError("That isn't a valid location!");
			}
		}
		
		private static void Examine(string itemName)
		{
			int itemId = World.GetItemIdByName(itemName);
			
			if (World.IsItem(itemName))
			{				
				if (World.Map[Location].ContainsItem(itemId) || ItemIsInInventory(itemId))
				{
					World.ItemDescription(itemId);
				}
				else
				{
					Program.SetError("That item isn't here!");
				}
			}
			else
			{
				Program.SetError("That isn't a valid item!");
			}
		}
		
		private static void Pickup(string itemName)
		{
			if (World.IsItem(itemName))
			{
				int itemId = World.GetItemIdByName(itemName);
				
				if (World.Map[Location].ContainsItem(itemId))
				{
					World.Map[Location].Items.Remove(itemId);
					inventory.Add(itemId);
					Program.SetNotification("You put the " + World.Items[itemId].Name + " in your bag!");
				}
				else
				{
					Program.SetError("That item isn't here!");
				}
			}
			else
			{
				Program.SetError("That isn't a valid item!");
			}
		}
		
		private static void Place(string itemName)
		{
			if (World.IsItem(itemName))
			{
				int itemId = World.GetItemIdByName(itemName);
				
				if (ItemIsInInventory(itemId))
				{
					inventory.Remove(itemId);
					World.Map[Location].Items.Add(itemId);
					Program.SetNotification("You placed the " + World.Items[itemId].Name + " in the room!");
				}
				else
				{
					Program.SetError("You aren't holding that item!");
				}
			}
			else
			{
				Program.SetError("That isn't a valid item!");
			}
		}
		
		private static void Use(string itemName)
		{
			if (World.IsItem(itemName))
			{
				int itemId = World.GetItemIdByName(itemName);
				
				if (ItemIsInInventory(itemId))
				{
					World.Items[itemId].Use();
				}
			}
		}
		
		private static void DisplayInventory()
		{
			if (inventory.Count <= 0)
			{
				Program.SetNotification("You aren't carrying anything!");
				return;
			}
				
			Text.WriteColor("`m`/---------------\\");
			Text.WriteColor("|`w`   Inventory   `m`|");
			Text.WriteLine(">---------------<");
			
			for (int i = 0; i < inventory.Count; i++)
			{
				string name = World.Items[inventory[i]].Name;
				Text.WriteColor("|`w`" + name, false);
				Text.BlankSpaces(15 - name.Length);
				Text.WriteColor("`m`|");
			}
			
			Text.WriteColor("\\---------------/`w`");
			Text.BlankLines();
		}
		#endregion
		
		#region Helper Methods
		private static bool IsValidInput(string[] type, string aString, bool notify)
		{
			for (int i = 0; i < type.Length; i++)
			{
				if (type[i] == aString)
					return true;
			}
			
			if(notify)
			{
				Program.SetError("That isn't a valid command!");
			}
			
			return false;
		}
		
		private static bool ItemIsInInventory(int itemId)
		{
			if (inventory.Contains(itemId))
				return true;
			else
				return false;
		}
		#endregion
		
		#region Public Methods
		public static void RemoveItemFromInventory(int itemId)
		{
			if (ItemIsInInventory(itemId))
			{
				inventory.Remove(itemId);
			}
		}
		#endregion
	}
}