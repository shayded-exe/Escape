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

		public static int Power = 10;
		public static int Defense = 10;

		public static int Level = 1;
		public static int Exp = 0;
		public static int NextLevel = 10;
		
		public static List<int> Inventory = new List<int>();
		public static List<int> Attacks = new List<int>();
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
						case "item":
							Use(noun);
							break;
						case "items":
						case "inventory":
						case "inv":
							DisplayInventory();
							break;
						case "attack":
							Attack(noun);
							break;
						case "hurt":
							Player.Health -= Convert.ToInt32(noun);
							break;
						case "exp":
							AddExp(Convert.ToInt32(noun));
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
						case "help":
						case "?":
							WriteBattleCommands();
							break;
						case "attack":
							AttackInBattle(noun);
							break;
						case "flee":
						case "escape":
						case "run":
							//flee command
							break;
						case "use":
						case "item":
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

		public static List<int> ItemsUsableInBattle()
		{
			List<int> result = new List<int>();

			foreach (int item in Inventory)
			{
				if (World.Items[item].CanUseInBattle)
				{
					result.Add(item);
				}
			}

			return result;
		}

		public static void AddExp(int expAmount)
		{
			Player.Exp += expAmount;

			while (Player.Exp >= NextLevel)
			{
				Player.Level++;
				Player.Exp -= NextLevel;
				GenerateNextLevel();
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
			Text.WriteColor("use/item <`c`item`w`> - Use the specified item.");
			Text.WriteColor("attack <`c`enemy`w`> - Attack the specified enemy.");
			Text.WriteColor("save/load - saves/loads the game respectively.");
			Text.BlankLines();
		}
		
		private static void MoveTo(string locationName)
		{
			if (World.IsLocation(locationName))
			{
				int locationId = World.GetLocationIDByName(locationName);
				
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
			int itemId = World.GetItemIDByName(itemName);
			
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
				int itemId = World.GetItemIDByName(itemName);
				
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
				int itemId = World.GetItemIDByName(itemName);
				
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
				int itemId = World.GetItemIDByName(itemName);
				
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

		private static void Attack(string enemyName)
		{
			if (World.IsEnemy(enemyName))
			{
				int enemyId = World.GetEnemyIDByName(enemyName);

				if (World.Map[Location].ContainsEnemy(enemyId))
				{
					BattleCore.StartBattle(enemyId);
					Program.SetNotification("You attacked the " + World.Enemies[enemyId].Name + ". Prepare for battle!");
				}
				else
				{
					Program.SetError("That enemy isn't able to take your call at the moment, please leave a message!..... **BEEP**");
				}
			}
			else
			{
				Program.SetError("That isn't a valid enemy!");
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

		#region Battle Command Methods
		private static void WriteBattleCommands()
		{
			Text.WriteColor("`g`Available Battle Commands:`w`");
			Text.WriteColor("help/? - Display this list.");
		}

		private static void AttackInBattle(string attackName)
		{
			if (World.IsAttack(attackName))
			{
				int attackID = World.GetAttackIDByName(attackName);

				if (AttackIsInInventory(attackID))
				{
					World.Attacks[attackID].Use();
				}
				else
				{
					Program.SetError("You don't know that attack!");
				}
			}
			else
			{
				Program.SetError("That isn't a valid attack!");
			}
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

		private static bool AttackIsInInventory(int attackId)
		{
			if (Attacks.Contains(attackId))
				return true;
			else
				return false;
		}

		private static void GenerateNextLevel()
		{
			NextLevel = (int)Math.Pow(Level, 3);

			if (Player.Level < 5)
			{
				NextLevel += 10;
			}
		}
		#endregion
	}
}