using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Escape
{
    class Attack : INamed
    {
        #region Definitions
        public enum AttackType
        {
            None,
            Physical,
            Magic,
            Self
        };
        #endregion

        #region Declarations
        public string Name { get; set; }
        public string Description;

        public int Power;
        public int Accuracy;
        public int Cost;
        public AttackType Type;

        public Attack Fallback;
        #endregion

        #region Constructor
        public Attack(
            string name,
            // A lot of these defaults probably aren't reasonable.
            // Make them required if you think they should be set in almost all cases,
            // that way you'll err slightly towards the side of safety.
            string description,
            int power,
            int accuracy,
            int cost,
            AttackType type = AttackType.None,
            Attack fallback = null)
        {
            // The assertion here isn't 100% foolproof,
            // weird things (errors) happen when someone's magic pool is negative.
            if (fallback == null && cost > 0)
            {
                throw new ArgumentNullException("fallback", "A fallback must be specified if the cost is higher than 0.");
            }

            this.Name = name;
            this.Description = description;
            this.Power = power;
            this.Accuracy = accuracy;
            this.Cost = cost;
            this.Type = type;
            this.Fallback = fallback;
        }
        #endregion

        #region Public Methods
        // The messages here aren't perfect imo (not sure if it's an enemy).
        // Maybe there could be a method returning the proper name/noun phrase for the ICombatant.
        public void Use(ICombatant attacker, ICombatant defender)
        {
            if (attacker is Player)
            {
                Program.SetNotification("You used " + this.Name + "!");
            }
            else
            {
                Program.SetNotification("The enemy used " + this.Name + "!");
            }

            if (!CheckMagic(attacker))
            {
                Fallback.Use(attacker, defender);
            }

            attacker.Magic -= this.Cost;

            int lucky = CheckLucky(attacker);

            if (!CheckAccuracy(lucky, attacker))
                return;

            int damage = CheckDamage(lucky, attacker, defender);

            defender.Health -= damage;
        }
        #endregion

        #region Helper Methods
        private int CheckLucky(ICombatant attacker)
        {
            Random rand = Program.Random;
            int random = rand.Next(0, 100);

            double modifiedLuckyRate = BattleCore.BaseLuckyRate * (2 - (attacker.Health / (double)attacker.MaxHealth));

            if (random < modifiedLuckyRate)
            {
                Program.SetNotification("Lucky Hit!");
                return 2;
            }
            else
                return 1;
        }

        private bool CheckAccuracy(int lucky, ICombatant attacker)
        {
            if (lucky == 2)
            {
                return true;
            }

            Random rand = new Random();
            int random = rand.Next(0, 100);

            double modifiedAccuracy = this.Accuracy;

            if ((attacker.Health / (double)attacker.MaxHealth) < 0.5)
            {
                modifiedAccuracy = this.Accuracy * ConvertRange(0, 100, 80, 100, ((attacker.Health / (double)attacker.MaxHealth) * 200)) * 0.01;
            }

            if (random < modifiedAccuracy)
                return true;
            else
            {
                Program.SetError("The attack missed!");
                return false;
            }

        }

        private bool CheckMagic(ICombatant attacker)
        {
            if (attacker.Magic >= this.Cost)
                return true;
            else
            {
                if (attacker is Player)
                {
                    Program.SetError("Not enough magic! You flailed your arms.");
                }
                else
                {
                    Program.SetError("The enemy is out of magic! It flailed its arms.");
                }
                return false;
            }
        }

        private int CheckDamage(int lucky, ICombatant attacker, ICombatant defender)
        {
            Random random = Program.Random;
            double modifier = lucky * 0.6;
            double variation = (random.Next(85, 115) / 100d);

            double damage = ((attacker.Power * this.Power) / (double)Math.Max(defender.Defense, 1)) * modifier * variation;

            if (attacker is Player)
            {
                Program.SetNotification("You did " + (int)damage + " damage!");
            }
            else
            {
                Program.SetNotification("The enemy did " + (int)damage + " damage");
            }

            return (int)damage;
        }

        public static int ConvertRange(
            int originalStart, int originalEnd,
            int newStart, int newEnd,
            double value)
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }
        #endregion
    }
}
