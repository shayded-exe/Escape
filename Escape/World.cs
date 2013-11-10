using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Escape
{
    // This better be something...
    // Some sort of abomination between a Dictionary and a List.
    // Basically a wrapper so we don't need all those "GetSomethingByID" and "GetSomethingByName" methods,
    // and neither need to do all those conversions.
    //
    // This collection may contain it all.
    /// <summary>
    /// A collection wrapper to store values with string and ID keys.
    /// </summary>
    /// <typeparam name="TEntry">Type of the value</typeparam>
    public class EntryDatabase<TEntry>
    {
        #region Declarations
        private List<TEntry> _BackingList; // Storage for elements
        private Dictionary<string, int> _Index; // Link between the key and the value
        #endregion

        #region Constructor
        /// <summary>
        /// Set up a new EntryDatabase with the specified key and value type.
        /// </summary>
        public EntryDatabase()
        {
            this._BackingList = new List<TEntry>();
            this._Index = new Dictionary<string, int>();

            Console.WriteLine("Initialized.");
        }
        #endregion

        #region Modifiers
        /// <summary>
        /// Add an entry to the collection
        /// </summary>
        public void Add(string key, TEntry value)
        {
            if (this.Contains(key.ToLowerInvariant()))
                throw new ArgumentException("An item with the same key has already been added.");

            _BackingList.Add(value);
            _Index.Add(key.ToLowerInvariant(), _BackingList.IndexOf(value));
        }

        /// <summary>
        /// Remove the entry with the specified value
        /// </summary>
        public void Remove(TEntry value)
        {
            if(this.Contains(value))
            {
                int entryIndex = _BackingList.IndexOf(value);

                // Remove the entry itself
                _BackingList.RemoveAt(entryIndex);

                // Remove link from index
                // The center line searches for the TPrimaryKey for the entry's index
                _Index.Remove(
                    _Index.First(e => e.Value == entryIndex).Key.ToLowerInvariant()
                );
            }
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Remove the entry at the specified index
        /// </summary>
        public void Remove(int index)
        {
            try
            {
                // Delete the item
                _BackingList.RemoveAt(index);

                // Remove link from index
                _Index.Remove(
                    _Index.First(e => e.Value == index).Key.ToLowerInvariant()
                );
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the index of the element we want to delete is out of range, rethrow the exception.
                throw;
            }
        }

        /// <summary>
        /// Remove the entry with the specified key
        /// </summary>
        public void Remove(string key)
        {
            try
            {
                // Fetch the index for the key and remove the value
                _BackingList.RemoveAt(
                    _Index[key.ToLowerInvariant()]
                );

                // Update the index
                _Index.Remove(key.ToLowerInvariant());
            }
            catch (ArgumentOutOfRangeException)
            {
                // If the key of the element we want to delete is out of range, rethrow the exception.
                throw;
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get the entry based on its key
        /// </summary>
        public TEntry this[string key]
        {
            get
            {
                return _BackingList[
                    _Index[key.ToLowerInvariant()]
                ];
            }
        }

        /// <summary>
        /// Get the entry based on its ID
        /// </summary>
        public TEntry this[int id]
        {
            get
            {
                return _BackingList[id];
            }
        }
        #endregion

        #region Lookups
        /// <summary>
        /// The count of elements in the collection
        /// </summary>
        public int Count
        {
            get
            {
            return _BackingList.Count;
            }
        }

        /// <summary>
        /// Get the index associanted with the value
        /// </summary>
        public int IndexOf(TEntry value)
        {
            if(this.Contains(value))
                 return _BackingList.IndexOf(value);
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Get the key for a specified value
        /// </summary>
        public string KeyOf(TEntry value)
        {
            if (this.Contains(value))
            {
                int entryIndex = _BackingList.IndexOf(value);

                // Find the key for the entry's index in the Dictionary
                return _Index.Where(i => i.Value == entryIndex).Select(s => s.Key).First().ToLowerInvariant();
            }
            else
                throw new ArgumentException("Value is not in collection");
        }

        /// <summary>
        /// Get whether the specified value exits in the collection
        /// </summary>
        public bool Contains(TEntry value)
        {
            return _BackingList.Contains(value);
        }

        /// <summary>
        /// Get whether a value with the specified index exits in the collection
        /// </summary>
        public bool Contains(int index)
        {
            try
            {
                // Trying to access the element at the specified index will throw an exception if missing.
                _BackingList.ElementAt(index);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get whether a value with the specified key exits in the collection
        /// </summary>
        public bool Contains(string key)
        {
            return _Index.ContainsKey(key.ToLowerInvariant());
        }
        #endregion

        #region Enumerators
        /// <summary>
        /// Get an IEnumerable containing the entries of the collection
        /// </summary>
        public IEnumerable<TEntry> GetEntries()
        {
            foreach (TEntry entry in _BackingList)
                yield return entry;
        }

        /// <summary>
        /// Get an IEnumerable containing KeyValuePair<>s where the Key is the numerical ID and the Value is the entry
        /// </summary>
        public IEnumerable<KeyValuePair<int, TEntry>> GetIDPairs()
        {
            foreach (KeyValuePair<string, int> indexEntry in _Index)
                yield return new KeyValuePair<int, TEntry>(indexEntry.Value, _BackingList[indexEntry.Value]);
        }

        /// <summary>
        /// Get an IEnumerable containing KeyValuePair<>s where the Key is the string key and the Value is the entry
        /// </summary>
        public IEnumerable<KeyValuePair<string, TEntry>> GetNamePairs()
        {
            foreach (KeyValuePair<string, int> indexEntry in _Index)
                yield return new KeyValuePair<string, TEntry>(indexEntry.Key, _BackingList[indexEntry.Value]);
        }
        #endregion
    }

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
                    if (Player.Location == World.GetLocationIDByName((string)self.ExtendedAttributes["str_targetLocation"]))
                    {
                        Program.SetNotification("The " + self.Name + " opened the lock!");
                        /*World.Map[World.GetLocationIDByName((string)self.ExtendedAttributes["str_targetLocation"])].Exits.Add(
                            World.GetLocationIDByName((string)self.ExtendedAttributes["str_newLocation"]));*/
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
                    if (Player.Location == World.GetLocationIDByName("secret room"))
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