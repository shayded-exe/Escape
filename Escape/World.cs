using Escape.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Escape
{
    static class World
    {
        #region Declarations
        // Creates lists that hold all the information about the world
        public static EntryDatabase<Location> Locations = new EntryDatabase<Location>(ExtractDefinitions<Location>(typeof(Locations)), x => x.Name);
        public static EntryDatabase<Item> Items = new EntryDatabase<Item>(ExtractDefinitions<Item>(typeof(Items)), x => x.Name);
        public static EntryDatabase<Enemy> Enemies = new EntryDatabase<Enemy>(ExtractDefinitions<Enemy>(typeof(Enemies)), x => x.Name);
        public static EntryDatabase<Attack> Attacks = new EntryDatabase<Attack>(ExtractDefinitions<Attack>(typeof(Attacks)), x => x.Name);
        #endregion

        #region Initialization

        private static IEnumerable<T> ExtractDefinitions<T>(Type definitionClassType)
        {
            // What this does is retrieve the names and values of all public fields
            // in the definitions classes, filtered by the type of the field.
            return
                from field in definitionClassType.GetFields(BindingFlags.Public | BindingFlags.Static)
                where field.FieldType == typeof(T)
                select (T)field.GetValue(null);
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