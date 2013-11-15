using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Definitions
{
    static class Enemies
    {
        public static readonly Enemy Rat = new Enemy(
            name: "Rat",
            description: "It's just a pwesious wittle wat that will KILL YOU!",
            health: 10, maxHealth: 10,
            magic: 5, maxMagic: 5,
            power: 10,
            defense: 5,
            expValue: 5,
            attacks: new[] { Attacks.Scratch });

        public static readonly Enemy Hawk = new Enemy(
            name: "Hawk",
            description: "It flies around looking for prey to feed on.",
            health: 15, maxHealth: 15,
            magic: 0, maxMagic: 0,
            power: 15,
            defense: 0,
            expValue: 8,
            attacks: new[] { Attacks.Scratch });
    }
}
