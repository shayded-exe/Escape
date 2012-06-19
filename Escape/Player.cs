using System;
using System.Collections.Generic;

namespace Escape
{
	static class Player
	{
		#region Declarations
		public static string Name;
		public static int Location = 0;
		
		public static int MaxHealth = 100;
		public static int Health = MaxHealth;

		public static int MaxMagic = 100;
		public static int Magic = MaxMagic;
		
		public static List<int> Inventory = new List<int>();
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
						case "hurt":
							Player.Health -= Convert.ToInt32(noun);
							break;
						case "save":
							Program.Save();
							break;
						case "load":
							Program.Load();
							break;
						default:
							InputNotValid();
							break;							
					}
					break;
					
				case Program.GameStates.Battle:
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
						default:
							InputNotValid();
							break;
					}
					break;
			}
		}
		
		public static void RemoveItemFromInventory(int itemId)
		{
			if (ItemIsInInventory(itemId))
			{
				Inventory.Remove(itemId);
			}
		}
		#endregion
		
		#region Command Methods
		private static void WriteCommands()
		{
			Text.WriteColor("`g`Available Commands:`w`");
			Text.WriteColor("help/? - Display this list.");
			Text.WriteColor("exit/quit - Exit the game.");
			Text.WriteColor("move/go <`c`location`w`> - Move to the specified location.");
			Text.WriteColor("examine <`c`item`w`> - Show info about the specified item.");
			Text.WriteColor("take/pickuip <`c`item`w`> - Put the specified item in your inventory.");
			Text.WriteColor("drop/place <`c`item`w`> - Drop the specified item from your inventory and place it in the world.");
			Text.WriteColor("items/inventory/inv - Display your current inventory.");
			Text.WriteColor("use <`c`item`w`> - Use the specified item.");
			Text.WriteColor("save/load - saves/loads the game respectively.");
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
					
					World.Map[Location].CalculateRandomBattle();
				}
				else if (Player.Location == locationId)
				{
					Program.SetError("You are already there!");
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
					Inventory.Add(itemId);
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
					Inventory.Remove(itemId);
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
		
		private static void DisplayInventory()
		{
			if (Inventory.Count <= 0)
			{
				Program.SetNotification("You aren't carrying anything!");
				return;
			}
				
			Text.WriteColor("`m`/-----------------\\");
			Text.WriteColor("|`w`    Inventory    `m`|");
			Text.WriteLine(">-----------------<");
			
			for (int i = 0; i < Inventory.Count; i++)
			{
				string name = World.Items[Inventory[i]].Name;
				Text.WriteColor("|`w` " + name + Text.BlankSpaces(16 - name.Length, true) + "`m`|");
			}
			
			Text.WriteColor("\\-----------------/`w`");
			Text.BlankLines();
		}
		#endregion
		
		#region Helper Methods
		private static void InputNotValid()
		{
			Program.SetError("That isn't a valid command!");
		}
		
		private static bool ItemIsInInventory(int itemId)
		{
			if (Inventory.Contains(itemId))
				return true;
			else
				return false;
		}
		#endregion
	}
}