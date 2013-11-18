using System;
using System.Collections.Generic;

namespace Escape
{
    class Player : ICombatant
    {
        #region Declarations
        // Possibly temporary
        public World World;

        private const int baseHealth = 100;
        private const int baseMagic = 100;

        private const int power = 15;
        private const int defense = 10;

        private string name;

        // Removed check whether location is in World,
        // it doesn't matter to the program and the code is simpler this way
        public Location Location { get; set; }

        private int level = 1;
        private int exp = 0;

        //QUESTION: Is this strange naming or a mistake?
        private int health = BattleCore.CalculateHealthStat(baseHealth, level: 1);
        private int magic = BattleCore.CalculateHealthStat(baseMagic, level: 1);

        public List<Item> Inventory = new List<Item>();
        public List<Attack> Attacks = new List<Attack>();
        #endregion

        #region Properties
        public string Name
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

        public event Action<Player> Died;

        public int Health
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
                    if (Died != null) { Died(this); }
                }
            }
        }

        public int MaxHealth
        {
            get
            {
                return BattleCore.CalculateHealthStat(baseHealth, level);
            }
        }

        public int Magic
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

        public int MaxMagic
        {
            get
            {
                return BattleCore.CalculateHealthStat(baseMagic, level);
            }
        }

        public int Level
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

        public int Exp
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

        public int Power
        {
            get
            {
                return BattleCore.CalculateStat(power, level);
            }
        }

        public int Defense
        {
            get
            {
                return BattleCore.CalculateStat(defense, level);
            }
        }
        #endregion

        #region Public Methods
        public void DoPlaying(string input, BattleCore battleCore)
        {
            string verb;
            string noun;
            SplitInputToLower(input, out verb, out noun);

            switch (verb)
            {
                case "help":
                case "?":
                    WriteCommands();
                    break;
                case "exit":
                case "quit":
                    Program.QuitState();
                    break;
                case "move":
                case "go":
                    MoveTo(noun, battleCore);
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
                    Attack(noun, battleCore);
                    break;
                case "hurt":
                    Health -= Convert.ToInt32(noun);
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
        }

        public void DoBattle(string input, BattleCore battleCore)
        {
            string verb;
            string noun;
            SplitInputToLower(input, out verb, out noun);

            switch (verb)
            {
                case "help":
                case "?":
                    WriteBattleCommands();
                    break;
                case "attack":
                    AttackInBattle(noun, battleCore);
                    break;
                case "flee":
                case "escape":
                case "run":
                    //flee command
                    break;
                case "use":
                case "item":
                    UseInBattle(noun, battleCore);
                    break;
                case "items":
                case "inventory":
                case "inv":
                    DisplayBattleInventory();
                    break;
                case "exit":
                case "quit":
                    Program.QuitState();
                    break;
                default:
                    {
                        // Moved attack check to player
                        AttackInBattle(verb, battleCore);
                        break;
                    }
            }
        }

        private static void SplitInputToLower(string aString, out string verb, out string noun)
        {
            verb = "";
            noun = "";

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
        }

        // Only look up item once
        public void GiveAttack(string attackName)
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

        public void GiveItem(string itemName)
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

        public void GiveExp(int amount)
        {
            Exp += amount;
        }

        public int GetNextLevel()
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

        private void MoveTo(string locationName, BattleCore battleCore)
        {
            Location exit;
            if (TryGetFromName(locationName, out exit, Location.Exits))
            {
                Location = exit;

                Location.CalculateRandomBattle(this, battleCore);
            }
            else if (Location.Name == locationName)
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

        private void Examine(string itemName)
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

        private void Pickup(string itemName)
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

        private void Place(string itemName)
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

        private void Use(string itemName)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Inventory))
            {
                item.Use(user: this);
            }
            else
            {
                //REWRITE
                //Program.SetError("You aren't holding that item!");
                //Program.SetError("That isn't a valid item!");
                Program.SetError("You aren't holding that item or that isn't a valid item!");
            }
        }

        private void Attack(string enemyName, BattleCore battleCore)
        {
            Enemy enemy;
            if (TryGetFromName(enemyName, out enemy, Location.Enemies))
            {
                battleCore.StartBattle(this, enemy);
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

        private void DisplayInventory()
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

        private void AttackInBattle(string attackName, BattleCore battleCore)
        {
            Attack attack;
            if (TryGetFromName(attackName, out attack, Attacks))
            {
                attack.Use(battleCore);
            }
            else
            {
                //REWRITE
                //Program.SetError("That isn't a valid attack!");
                //Program.SetError("You don't know that attack!");
                Program.SetError("You don't know that attack or that isn't a valid attack!");
            }
        }

        private void UseInBattle(string itemName, BattleCore battleCore)
        {
            Item item;
            if (TryGetFromName(itemName, out item, Inventory))
            {
                if (item.UsableInBattle)
                {
                    item.UseInBattle(this, battleCore.Defender);
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
        }

        private void DisplayBattleInventory()
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

        private void LevelUp()
        {
            exp -= GetNextLevel();
            Level++;
        }
        #endregion
    }
}
