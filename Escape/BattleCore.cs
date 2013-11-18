using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Escape
{
    class BattleCore
    {
        #region Declarations
        public ICombatant Attacker { get; set; }
        public ICombatant Defender { get; set; }

        public static double BaseLuckyRate = 7.5;
        #endregion

        public Action<BattleCore> BattleHandler { get; set; }

        #region Battle Methods
        public void StartBattle(ICombatant attacker, ICombatant defender)
        {
            // Maybe cloning should be a property of the combatant class
            if (attacker is Enemy)
            {
                attacker = CloneEnemy((Enemy)attacker);
            }
            if (defender is Enemy)
            {
                defender = CloneEnemy((Enemy)defender);
            }
            this.Attacker = attacker;
            this.Defender = defender;

            BattleHandler(this);
        }

        public void NextTurn()
        {
            // -snip- should have been unreachable

            //TODO: Unify
            if (Attacker is Player)
            {
                var player = (Player)Attacker;
                string temp = Text.SetPrompt("[" + player.Location.Name + "] > ");
                Text.Clear();
                player.DoBattle(temp, this);
            }
            else if (Attacker is Enemy)
            {
                Text.SetKeyPrompt("[Press any key to continue!]");
                Text.Clear();

                ((Enemy)Attacker).Attack(this);
            }
            else
            {
                Program.SetError("Errrr.... wanna fuk?");
            }

            // Swap sides
            var tempCombatant = Attacker;
            Attacker = Defender;
            Defender = tempCombatant;
        }

        public void CheckResults(out bool isEnd)
        {
            // The status messages should be moved into event handlers.
            var currentEnemy = Attacker as Enemy ?? (Enemy)Defender;
            var player = Attacker as Player ?? (Player)Defender;
            if (currentEnemy.Health <= 0) // I took out the second part of the condition because this should only be called during battles
            {
                Program.SetNotification("You defeated the " + currentEnemy.Name + " and gained " + currentEnemy.ExpValue + " EXP!");
                player.GiveExp(currentEnemy.ExpValue);
                player.Location.Enemies.Remove(currentEnemy);
                isEnd = true;
            }
            else
            {
                isEnd = false;
            }
        }
        #endregion

        #region Public Methods
        public void BattleHUD()
        {
            var currentEnemy = Attacker as Enemy ?? (Enemy)Defender;
            var player = Attacker as Player ?? (Player)Defender;

            Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);

            List<string> enemyDescription = Text.Limit(currentEnemy.Name + " - " + currentEnemy.Description, Console.WindowWidth - 4);

            foreach (string line in enemyDescription)
            {
                Text.WriteColor("| `w`" + line + Text.BlankSpaces((Console.WindowWidth - line.Length - 4), true) + "`c` |", false);
            }

            Text.WriteColor(">-----------------v-----------------v-----------------v-----------------<", false);
            Text.WriteColor("|      `w`Stats`c`      |     `w`Attacks`c`     |      `w`Items`c`      |      `w`Enemy`c`      |", false);
            Text.WriteColor(">-----------------#-----------------#-----------------#-----------------<`w`", false);

            int currentY = Console.CursorTop;
            int i;
            int longestList = 0;

            Text.WriteColor("  HP [`r`" + Text.ToBar(player.Health, player.MaxHealth, 10) + "`w`]");
            Text.WriteColor("  MP [`g`" + Text.ToBar(player.Magic, player.MaxMagic, 10) + "`w`]");

            longestList = (2 > longestList) ? 2 : longestList;
            i = 0;

            Console.SetCursorPosition(18, currentY);

            foreach (Attack attack in player.Attacks)
                Text.WriteColor("  " + attack.Name);

            longestList = (i > longestList) ? i : longestList;
            i = 0;

            Console.SetCursorPosition(36, currentY);

            foreach (Item item in player.Inventory.FindAll(j => j.UsableInBattle)) // Use 'j' instead of 'i' because 'i' is already used
                Text.WriteColor("  " + item.Name);

            longestList = (i > longestList) ? i : longestList;
            i = 0;

            Console.SetCursorPosition(54, currentY);

            Text.WriteColor("  HP [`r`" + Text.ToBar(currentEnemy.Health, currentEnemy.MaxHealth, 10) + "`w`]");
            Text.WriteColor("  MP [`g`" + Text.ToBar(currentEnemy.Magic, currentEnemy.MaxMagic, 10) + "`w`]");

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

            if (player.Level < 10)
            {
                Text.Write(" ");
            }

            Text.WriteColor(player.Level + " [`g`" + Text.ToBar(player.Exp, player.GetNextLevel(), 23) + "`w`] `c`|`w` "
            + player.Exp + "/" + player.GetNextLevel() + "`c` /", false);

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

        public static int CalculateStat(int baseStat, int level)
        {
            return (int)Math.Floor((2 * baseStat * level) / 100d + 5);
        }

        public static int CalculateHealthStat(int baseStat, int level)
        {
            return (int)Math.Floor((2 * baseStat * (level + 4)) / 100d + (level + 4) + 10);
        }
        #endregion

        #region Helper Methods
        private static Enemy CloneEnemy(Enemy enemy)
        {
            return new Enemy(
                enemy.Name,
                enemy.Description,
                enemy.Health,
                enemy.MaxHealth,
                enemy.Magic,
                enemy.MaxMagic,
                enemy.Power,
                enemy.Defense,
                enemy.ExpValue,
                new List<Attack>(enemy.Attacks));
        }
        #endregion
    }
}