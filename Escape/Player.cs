using System;
using System.Collections.Generic;

namespace Escape
{
    static class Player
    {
        #region Declarations
        public static World World;

        private const int baseHealth = 100;
        private const int baseMagic = 100;

        private const int power = 15;
        private const int defense = 10;

        private static string name;

        // Removed check whether location is in World,
        // it doesn't matter to the program and the code is simpler this way
        public static Location Location { get; set; }

        private static int level = 1;
        private static int exp = 0;

        private static int health = MaxHealth;
        private static int magic = MaxMagic;

        public static List<Item> Inventory = new List<Item>();
        public static List<Attack> Attacks = new List<Attack>();
        #endregion

        #region Properties
        public static string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != "")
                {
                    name = value;
                }
                else
                {
                    //Error: PL44
                    Program.SetError("Go tell the developer he dun goofed. Error: PL44");
                }
            }
        }

        public static int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = Math.Min(value, MaxHealth);

                if (health <= 0)
                {
                    Program.GameState = Program.GameStates.GameOver;
                }
            }
        }

        public static int MaxHealth
        {
            get
            {
                return BattleCore.CalculateHealthStat(baseHealth, level);
            }
        }

        public static int Magic
        {
            get
            {
                return magic;
            }
            set
            {
                magic = Math.Max(Math.Min(value, MaxMagic), 0);
            }
        }

        public static int MaxMagic
        {
            get
            {
                return BattleCore.CalculateHealthStat(baseMagic, level);
            }
        }

        public static int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        public static int Exp
        {
            get
            {
                return exp;
            }
            set
            {
                exp = value;

                while (exp >= GetNextLevel())
                {
                    LevelUp();
                }
            }
        }

        public static int Power
        {
            get
            {
                return BattleCore.CalculateStat(power, level);
            }
        }

        public static int Defense
        {
            get
            {
                return BattleCore.CalculateStat(defense, level);
            }
        }
        #endregion

        #region Public Methods
        public static void Do(string aString)
        {
            string verb = "";
            string noun = "";

            if (aString.IndexOf(" ") > 0)
            {
                string[] temp = aString.Split(new char[] { ' ' }, 2);
                verb = temp[0].ToLower();
                noun = temp[1].ToLower();
            }
            else
            {
                verb = aString.ToLower();
            }

            switch (Program.GameState)
            {
                case Program.GameStates.Playing:
                    switch (verb)
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
                            GiveExp(Convert.ToInt32(noun));
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
                    switch (verb)
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
                            UseInBattle(noun);
                            break;
                        case "items":
                        case "inventory":
                        case "inv":
                            DisplayBattleInventory();
                            BattleCore.CurrentTurn = "enemy";
                            break;
                        case "exit":
                        case "quit":
                            Program.GameState = Program.GameStates.Quit;
                            break;
                        default:
                            {
                                // Moved attack check to player
                                AttackInBattle(verb);

                                BattleCore.CurrentTurn = "enemy";
                                break;
                            }
                    }
                    break;
            }
        }

        // Only look up item once
        public static void GiveAttack(string attackName)
        {
            Attack attack;
            if (World.Attacks.TryGetValue(attackName, out attack))
            {
                Attacks.Add(attack);
                Program.SetNotification("You learned the attack " + attack.Name + "!");
            }
            else
            {
                //Error: PL149
                Program.SetError("Go tell the developer he dun goofed. Error: PL149");
            }
        }

        public static void GiveItem(string itemName)
        {
            Item item;
            if (World.Items.TryGetValue(itemName, out item))
            {
                Inventory.Add(item);
                Program.SetNotification("You were given " + Text.AorAn(item.Name));
            }
            else
            {
                //Error: PL177
                Program.SetError("Go tell the developer he dun goofed. Error: PL177");
            }
        }

        public static void GiveExp(int amount)
        {
            Exp += amount;
        }

        public static int GetNextLevel()
        {
            int result = (int)Math.Pow(Level, 3) + 10;

            if (Level < 5)
            {
                result += 10;
            }

            return result;
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
            Text.WriteColor("take/pickup <`c`item`w`> - Put the specified item in your inventory.");
            Text.WriteColor("drop/place <`c`item`w`> - Drop the specified item from your inventory and place it in the world.");
            Text.WriteColor("items/inventory/inv - Display your current inventory.");
            Text.WriteColor("use/item <`c`item`w`> - Use the specified item.");
            Text.WriteColor("attack <`c`enemy`w`> - Attack the specified enemy.");
            Text.WriteColor("save/load - saves/loads the game respectively.");
            Text.BlankLines();
        }

        private static void MoveTo(string locationName)
        {
            Location exit;
            if (TryGetFromName(locationName, out exit, Player.Location.Exits))
            {
                Player.Location = exit;

                Location.CalculateRandomBattle();
            }
            else if (Player.Location.Name == locationName)
            {
                Program.SetError("You are already there!");
            }
            else
            {
                //REWRITE
                //Program.SetError("This isn't a valid location!");
                //Program.SetError("You can't get there from here!");
                Program.SetError("This isn't a valid location or you can't get there from here!");
            }
        }

        private static void Examine(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Location.Items, Inventory))
            {
                Text.WriteLine(item.Description);
                Text.BlankLines();
            }
            else
            {
                //REWRITE
                //Program.SetError("That item isn't here!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("That item isn't here or that isn't a valid item!");
            }
        }

        private static void Pickup(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Location.Items))
            {
                Location.Items.Remove(item);
                Inventory.Add(item);
                Program.SetNotification("You put the " + item.Name + " in your bag!");
            }
            else
            {
                //REWRITE
                //Program.SetError("That item isn't here!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("That item isn't here or that isn't a valid item!");
            }
        }

        private static void Place(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Inventory))
            {
                Inventory.Remove(item);
                Location.Items.Add(item);
                Program.SetNotification("You placed the " + item.Name + " in the room!");
            }
            else
            {
                //REWRITE
                //Program.SetError("You aren't holding that item!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("You aren't holding that item or that isn't a valid item!");
            }
        }

        private static void Use(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Inventory))
            {
                item.Use();
            }
            else
            {
                //REWRITE
                //Program.SetError("You aren't holding that item!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("You aren't holding that item or that isn't a valid item!");
            }
        }

        private static void Attack(string enemyName)
        {
            Enemy enemy;
            if (TryGetFromName(enemyName, out enemy, Location.Enemies))
            {
                BattleCore.StartBattle(enemy);
                Program.SetNotification("You attacked the " + enemy.Name + ". Prepare for battle!");
            }
            else
            {
                //REWRITE
                //Program.SetError("That isn't a valid enemy!");
                //Program.SetError("That enemy isn't able to take your call at the moment, please leave a message!..... **BEEP**");
                Program.SetError("That isn't a valid enemy or hat enemy isn't able to take your call at the moment, please leave a message!..... **BEEP**");
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

            foreach (Item item in Inventory)
                Text.WriteColor("|`w` " + item.Name + Text.BlankSpaces(16 - item.Name.Length, true) + "`m`|");

            Text.WriteColor("\\-----------------/`w`");
            Text.BlankLines();
        }
        #endregion

        #region Battle Command Methods
        private static void WriteBattleCommands()
        {
            Text.WriteColor("`g`Available Battle Commands:`w`");
            Text.WriteColor("help/? - Display this list.");
            Text.BlankLines();
        }

        private static void AttackInBattle(string attackName)
        {
            Attack attack;
            if (TryGetFromName(attackName, out attack, Attacks))
            {
                attack.Use();
            }
            else
            {
                //REWRITE
                //Program.SetError("That isn't a valid attack!");
                //Program.SetError("You don't know that attack!");
                Program.SetError("You don't know that attack or that isn't a valid attack!");
            }
        }

        private static void UseInBattle(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Inventory))
            {
                if (item.UsableInBattle)
                {
                    item.UseInBattle(BattleCore.CurrentEnemy);
                    return;
                }
                else
                {
                    Program.SetError("You can't use that item in battle!");
                }
            }
            else
            {
                //REWRITE
                //Program.SetError("You aren't holding that item!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("You aren't holding that item or that isn't a valid item!");
            }

            BattleCore.CurrentTurn = "enemy";
        }

        private static void DisplayBattleInventory()
        {
            if (Inventory.Count <= 0)
            {
                Program.SetNotification("You aren't carrying anything!");
                return;
            }

            Text.WriteColor("`m`/-----------------\\");
            Text.WriteColor("|`w`    Inventory    `m`|");
            Text.WriteColor("|`w`   Battle Only   `m`|");
            Text.WriteLine(">-----------------<");

            foreach (Item item in Inventory.FindAll(i => i.UsableInBattle))
                Text.WriteColor("|`w` " + item.Name + Text.BlankSpaces(16 - item.Name.Length, true) + "`m`|");

            Text.WriteColor("\\-----------------/`w`");
            Text.BlankLines();
        }
        #endregion

        #region Helper Methods
        private static bool TryGetFromName<T>(
            string name,
            out T result,
            params IEnumerable<T>[] collections)
            where T : INamed
        {
            foreach (var c in collections)
            {
                foreach (var i in c)
                {
                    if (i.Name.ToLower() == name)
                    {
                        result = i;
                        return true;
                    }
                }
            }
            result = default(T);
            return false;
        }
        private static void InputNotValid()
        {
            Program.SetError("That isn't a valid command!");
        }

        private static void LevelUp()
        {
            exp -= GetNextLevel();
            Level++;
        }
        #endregion
    }
}
