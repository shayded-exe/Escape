using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape.Definitions
{
    static class Attacks
    {
        public static readonly Attack Flail = new Attack(
            name: "Flail",
            description: "Flail your arms like a fish out of water and hope something happens",
            power: 5,
            accuracy: 70,
            cost: 0,
            type: Attack.AttackType.Physical);

        public static readonly Attack Scratch = new Attack(
            name: "Scratch",
            description: "The Attacker digs its claws into the skin of its prey. Not really as painful as it sounds.",
            power: 10,
            accuracy: 70,
            cost: 1,
            type: Attack.AttackType.Physical);
    }
}
