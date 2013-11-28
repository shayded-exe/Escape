using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Escape
{
    // There probably won't be a good reason to keep it static in the long run, so I changed it.
    // This also helps with loading, as the previous state can simply be discarded and won't interfere with the new one.
    class World
    {
        #region Definitions
        public Location StartLocation { get; private set; }

        // This should be an IReadOnlyList<Attack>, but that is only available in .NET 4.5 or later.
        public Attack[] PlayerAttacks { get; private set; }
        #endregion

        #region Initialization
        public World()
        {
            // Location declarations
            // These appear here because the variables are caught in lambdas.
            Location
                room1 = null,
                room2 = null,
                room3 = null,
                secretRoom = null;

            // Attacks
            var flail = new Attack(
                name: "Flail",
                description: "Flail your arms like a fish out of water and hope something happens",
                power: 5,
                accuracy: 70,
                cost: 0,
                type: Attack.AttackType.Physical);

            var scratch = new Attack(
                name: "Scratch",
                description: "The Attacker digs its claws into the skin of its prey. Not really as painful as it sounds.",
                power: 10,
                accuracy: 70,
                cost: 1,
                type: Attack.AttackType.Physical,
                fallback: flail);

            //CHANGE: I'm giving the player a few attacks to start with so I can test better.
            PlayerAttacks = new[] { flail, scratch };

            // Enemies
            var rat = new Enemy(
                name: "Rat",
                description: "It's just a pwesious wittle wat that will KILL YOU!",
                health: 10, maxHealth: 10,
                magic: 5, maxMagic: 5,
                power: 10,
                defense: 5,
                expValue: 5,
                attacks: new[] { scratch });

            var hawk = new Enemy(
                name: "Hawk",
                description: "It flies around looking for prey to feed on.",
                health: 15, maxHealth: 15,
                magic: 0, maxMagic: 0,
                power: 15,
                defense: 0,
                expValue: 8,
                attacks: new[] { scratch });

            // Items
            var brassKey = new Item(
                name: "Brass Key",
                description: "Just your generic key that's in almost every game.",
                uses: (user, self) =>
                    {
                        var targetLocation = room3;
                        var newLocation = secretRoom;
                        if (user.Location == targetLocation)
                        {
                            Program.SetNotification("The " + self.Name + " opened the lock!");
                            targetLocation.Exits.Add(newLocation);
                        }
                        else
                            self.NoUse();
                    });

            var shinyStone = new Item(
                name: "Shiny Stone",
                description: "It's a stone and it's shiny, what more could you ask for?",
                uses: (user, self) => // Can be (Item self), but that's not necessary due to type inference.
                    {
                        if (user.Location == secretRoom)
                        {
                            user.Health += Math.Min(user.MaxHealth / 10, user.MaxHealth - user.Health);
                            Program.SetNotification("The magical stone restored your health by 10%!");
                        }
                        else
                            Program.SetNotification("The shiny stone glowed shiny colors!");
                    });

            var rock = new Item(
                name: "Rock",
                description: "It doesn't do anything, however, it is said that the mystical game designer used this for testing.",
                uses: (user, self) =>
                    {
                        Program.SetNotification("You threw the rock at a wall. Nothing happened.");
                    },
                battleUses: (user, self, victim) =>
                    {
                        Program.SetNotification("The rock hit the enemy in the head! It seems confused...");
                    });

            // Location initialization
            room1 = new Location(
                name: "Room 1",
                description: "This is a room.",
                unboundExits: new Func<Location>[] { () => room2 },
                items: new[]
                        {
                            brassKey,
                            rock
                        });
            StartLocation = room1;

            room2 = new Location(
                name: "Room 2",
                description: "This is another room.",
                unboundExits: new Func<Location>[]
                        {
                            () => room1,
                            () => room3
                        },
                items: new[] { shinyStone },
                enemies: new[] { rat },
                battleChance: 50);

            room3 = new Location(
                name: "Room 3",
                description: "This is yet another room.",
                unboundExits: new Func<Location>[] { () => room2 },
                enemies: new[]
                        {
                            rat,
                            hawk
                        },
                battleChance: 75);

            secretRoom = new Location(
                name: "Secret Room",
                description: "This is a very awesome secret room.",
                unboundExits: new Func<Location>[] { () => room3 });
        }

        #endregion

        #region Public Location Methods

        //Prints the main HUD that is displayed for most of the game. Warning, this gets a little complex.
        public static void LocationHUD(Player player)
        {
            Text.WriteColor("`c`/-----------------------------------------------------------------------\\", false);

            List<string> locationDesctiption = Text.Limit(player.Location.Name + " - " + player.Location.Description, Console.WindowWidth - 4);

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

            foreach (Location exit in player.Location.Exits)
                Text.WriteColor("  " + exit.Name);

            longestList = (i > longestList) ? i : longestList;
            i = 0;

            Console.SetCursorPosition(18, currentY);

            foreach (Item item in player.Location.Items)
                Text.WriteColor("  " + item.Name);

            longestList = (i > longestList) ? i : longestList;
            i = 0;

            Console.SetCursorPosition(36, currentY);

            foreach (Enemy enemy in player.Location.Enemies)
                Text.WriteColor("  " + enemy.Name);

            longestList = (i > longestList) ? i : longestList;
            i = 0;

            Console.SetCursorPosition(54, currentY);

            Text.WriteColor("  HP [`r`" + Text.ToBar(player.Health, player.MaxHealth, 10) + "`w`]");
            Text.WriteColor("  MP [`g`" + Text.ToBar(player.Magic, player.MaxMagic, 10) + "`w`]");

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
        #endregion
    }
}