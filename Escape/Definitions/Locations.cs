using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Definitions
{
    static class Locations
    {
        public static readonly Location Room1 = new Location(
            name: "Room 1",
            description: "This is a room.",
            unboundExits: new Func<Location>[] { () => Room2 },
            items: new[]
                {
                    Items.BrassKey,
                    Items.Rock
                });

        public static readonly Location Room2 = new Location(
            name: "Room 2",
            description: "This is another room.",
            unboundExits: new Func<Location>[]
                {
                    () => Room1,
                    () => Room3
                },
            items: new[] { Items.ShinyStone },
            enemies: new[] { Enemies.Rat },
            battleChance: 50);

        public static readonly Location Room3 = new Location(
            name: "Room 3",
            description: "This is yet another room.",
            unboundExits: new Func<Location>[] { () => Room2 },
            enemies: new[]
                {
                    Enemies.Rat,
                    Enemies.Hawk
                },
            battleChance: 75);

        public static readonly Location SecretRoom = new Location(
            name: "Secret Room",
            description: "This is a very awesome secret room.",
            unboundExits: new Func<Location>[] { () => Room3 });
    }
}
