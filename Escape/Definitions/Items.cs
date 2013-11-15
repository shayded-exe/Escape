using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Definitions
{
    static class Items
    {
        // Readonly prevents accidental changes.
        // The field can't be const due to Item instances not being compile time constants.
        public static readonly Item BrassKey = new Item(
            name: "Brass Key",
            description: "Just your generic key that's in almost every game.",
            extendedAttributes: new Dictionary<string, object>
                {
                    //TODO: This is unnecessary, replace with direct reference.
                    {"str_targetLocation", "room 3"},
                    {"str_newLocation", "secret room"}
                },
            uses: (self) =>
            {
                if (Player.Location == World.Locations[(string)self.ExtendedAttributes["str_targetLocation"]])
                {
                    Program.SetNotification("The " + self.Name + " opened the lock!");
                    World.Locations[(string)self.ExtendedAttributes["str_targetLocation"]].AddExit(
                        World.Locations[(string)self.ExtendedAttributes["str_newLocation"]]);
                }
                else
                    self.NoUse();
            });
        public static readonly Item ShinyStone = new Item(
            name: "Shiny Stone",
            description: "It's a stone and it's shiny, what more could you ask for?",
            uses: (self) => // Can be (Item self), but that's not necessary due to type inference.
                {
                    if (Player.Location == World.Locations["Secret Room"])
                    {
                        Player.Health += Math.Min(Player.MaxHealth / 10, Player.MaxHealth - Player.Health);
                        Program.SetNotification("The magical stone restored your health by 10%!");
                    }
                    else
                        Program.SetNotification("The shiny stone glowed shiny colors!");
                });

        public static readonly Item Rock = new Item(
            name: "Rock",
            description: "It doesn't do anything, however, it is said that the mystical game designer used this for testing.",
            uses: (self) =>
                {
                    Program.SetNotification("You threw the rock at a wall. Nothing happened.");
                },
            battleUses: (self, victim) =>
                {
                    Program.SetNotification("The rock hit the enemy in the head! It seems confused...");
                });
    }
}
