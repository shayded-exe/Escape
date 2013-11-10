using System;
using System.Collections.Generic;

namespace Escape
{
    static class World
	{
		#region Declarations
		//Creates empty lists to hold all the information about the world
        public static EntryDatabase<Location> Locations = new EntryDatabase<Location>();
        public static EntryDatabase<Item> Items = new EntryDatabase<Item>();
        public static EntryDatabase<Enemy> Enemies = new EntryDatabase<Enemy>();
        public static EntryDatabase<Attack> Attacks = new EntryDatabase<Attack>();
		#endregion
		
		#region Initialization
		//Generates the data that goes into the above lists
		public static void Initialize()
		{
            DefineIndex();
			GenerateWorld();
			GenerateItems();
			GenerateEnemies();
			GenerateAttacks();
		}
		#endregion
		
		#region World Generation Methods
        /*
         * Creates every world object's variable and assigns them to the database.
         */
        private static void DefineIndex()
        {
            // Locations
            Locations.Add("Room 1", new Location("Room 1"));
            Locations.Add("Room 2", new Location("Room 2"));
            Locations.Add("Room 3", new Location("Room 3"));
            Locations.Add("Secret Room", new Location("Secret Room"));

            // Items
            Items.Add("Brass Key", new Item("Brass Key"));
            Items.Add("Shiny Stone", new Item("Shiny Stone"));
            Items.Add("Rock", new Item("Rock"));

            // Enemies
            Enemies.Add("Rat", new Enemy("Rat"));
            Enemies.Add("Hawk", new Enemy("Hawk"));

            // Attacks
            Attacks.Add("Flail", new Attack("Flail"));
            Attacks.Add("Scratch", new Attack("Scratch"));
        }

		/*
		 * This defines all the locations that exist in the map along with their specific properties
		 */
		private static void GenerateWorld()
		{
            // Room 1
            Locations["Room 1"].Description = "This is a room.";
            Locations["Room 1"].AddExit(Locations["Room 2"]);
            Locations["Room 1"].AddItem(Items["Brass Key"]);
            Locations["Room 1"].AddItem(Items["Rock"]);

            // Room 2
            Locations["Room 2"].Description = "This is another room.";
            Locations["Room 2"].AddExit(Locations["Room 1"]);
            Locations["Room 2"].AddExit(Locations["Room 3"]);
            Locations["Room 2"].AddItem(Items["Shiny Stone"]);
            Locations["Room 2"].AddEnemy(Enemies["Rat"]);
            Locations["Room 2"].BattleChance = 50;

            // Room 3
            Locations["Room 3"].Description = "This is yet another room.";
            Locations["Room 3"].AddExit(Locations["Room 2"]);
            Locations["Room 3"].AddEnemy(Enemies["Rat"]);
            Locations["Room 3"].AddEnemy(Enemies["Hawk"]);
            Locations["Room 3"].BattleChance = 75;

            // Secret room
            Locations["Secret Room"].Description = "This is a very awesome secret room.";
            Locations["Secret Room"].AddExit(Locations["Room 3"]);
		}

		/*
		 * This defines all the items exist in the game.
		 */
		private static void GenerateItems()
		{
            // Brass Key
            Items["Brass Key"].Description = "Just your generic key that's in almost every game.";
            Items["Brass Key"].Usable = true;
            Items["Brass Key"].ExtendedAttributes.Add("str_targetLocation", "room 3");
            Items["Brass Key"].ExtendedAttributes.Add("str_newLocation", "secret room");
            Items["Brass Key"].Uses += new Item.OnUse(delegate(Item self)
                {
                    if (Player.Location == Locations[(string)self.ExtendedAttributes["str_targetLocation"]])
                    {
                        Program.SetNotification("The " + self.Name + " opened the lock!");
                        Locations[(string)self.ExtendedAttributes["str_targetLocation"]].AddExit(
                            Locations[(string)self.ExtendedAttributes["str_newLocation"]]);
                    }
                    else
                        self.NoUse();
                });

            // Shiny Stone
            Items["Shiny Stone"].Description = "It's a stone and it's shiny, what more could you ask for?";
            Items["Shiny Stone"].Usable = true;
            Items["Shiny Stone"].UsableInBattle = true;
            Items["Shiny Stone"].Uses += new Item.OnUse(delegate(Item self)
                {
                    if (Player.Location == World.Locations["Secret Room"])
                    {
                        Player.Health += Math.Min(Player.MaxHealth / 10, Player.MaxHealth - Player.Health);
                        Program.SetNotification("The magical stone restored your health by 10%!");
                    }
                    else
                        Program.SetNotification("The shiny stone glowed shiny colors!");
                });

            // Rock
            Items["Rock"].Description =
                "It doesn't do anything, however, it is said that the mystical game designer used this for testing.";
            Items["Rock"].Usable = true;
            Items["Rock"].UsableInBattle = true;
            Items["Rock"].Uses += new Item.OnUse(delegate(Item self)
                {
                    Program.SetNotification("You threw the rock at a wall. Nothing happened.");
                });
            Items["Rock"].BattleUses += new Item.OnUseInBattle(delegate(Item self, Enemy victim)
                {
                    Program.SetNotification("The rock hit the enemy in the head! It seems confused...");
                });
		}
		
		/*
		 * This defines all the enemies that exist in the game
		 */
		private static void GenerateEnemies()
		{
            // Rat
            Enemies["Rat"].Description = "It's just a pwesious wittle wat that will KILL YOU!";
            Enemies["Rat"].Health = 10; Enemies["Rat"].MaxHealth = 10;
            Enemies["Rat"].Magic = 5; Enemies["Rat"].MaxMagic = 5;
            Enemies["Rat"].Power = 10;
            Enemies["Rat"].Defense = 5;
            Enemies["Rat"].ExpValue = 5;
            Enemies["Rat"].AddAttack(Attacks["Scratch"]);

            // Hawk
            Enemies["Hawk"].Description = "It flies around looking for prey to feed on.";
            Enemies["Hawk"].Health = 15; Enemies["Hawk"].MaxHealth = 15;
            Enemies["Hawk"].Magic = 0; Enemies["Hawk"].MaxMagic = 0;
            Enemies["Hawk"].Power = 15;
            Enemies["Hawk"].Defense = 0;
            Enemies["Hawk"].ExpValue = 8;
            Enemies["Hawk"].AddAttack(Attacks["Scratch"]);
		}

		/*
		 * This defines all the enemies that exist in the game
		 */
		private static void GenerateAttacks()
		{
            // Flail
            Attacks["Flail"].Description = "Flail your arms like a fish out of water and hope something happens";
            Attacks["Flail"].Power = 5;
            Attacks["Flail"].Accuracy = 70;
            Attacks["Flail"].Cost = 0;
            Attacks["Flail"].Type = Attack.AttackType.Physical;

            // Scratch
            Attacks["Scratch"].Description = "The Attacker digs its claws into the skin of its prey. Not really as painful as it sounds.";
            Attacks["Scratch"].Power = 10;
            Attacks["Scratch"].Accuracy = 70;
            Attacks["Scratch"].Cost = 1;
            Attacks["Scratch"].Type = Attack.AttackType.Physical;
		}
		#endregion

		#region Public Location Methods

		//Prints the main HUD that is displayed for most of the game. Warning, this gets a little complex.
		public static void LocationHUD()
		{
			Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);
			
			List<string> locationDesctiption = Text.Limit(Player.Location.Name + " - " + Player.Location.Description, Console.WindowWidth - 4);
			
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
			
			foreach (Location exit in Player.Location.Exits)
				Text.WriteColor("  " + exit.Name);
			
			longestList = (i > longestList) ? i : longestList;
			i = 0;
			
			Console.SetCursorPosition(18, currentY);
			
			foreach (Item item in Player.Location.Items)
				Text.WriteColor("  " + item.Name);
			
			longestList = (i > longestList) ? i : longestList;
			i = 0;
			
			Console.SetCursorPosition(36, currentY);
			
			foreach (Enemy enemy in Player.Location.Enemies)
				Text.WriteColor("  " + enemy.Name);
			
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
	}
}