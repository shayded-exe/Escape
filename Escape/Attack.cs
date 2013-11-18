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
            string description = "",
            int power = 0,
            int accuracy = 0,
            int cost = 0,
            AttackType type = AttackType.None,
            Attack fallback = null)
        {
            // The assertion here isn't 100% foolproof,
            // weird things (errors) happen when someone's magic pool is negative.
            Debug.Assert(fallback != null || cost <= 1);

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
        public void Use()
        {
            if (BattleCore.CurrentTurn == "player")
            {
                Program.SetNotification("You used " + this.Name + "!");
            }
            else
            {
                Program.SetNotification("The enemy used " + this.Name + "!");
            }

            if (!CheckMagic())
            {
                Fallback.Use();
                return;
            }

            BattleCore.AttackerMagic -= this.Cost;

            int lucky = CheckLucky();

            if (!CheckAccuracy(lucky))
                return;

            int damage = CheckDamage(lucky);

            BattleCore.DefenderHealth -= damage;
        }
        #endregion

        #region Helper Methods
        private int CheckLucky()
        {
            Random rand = Program.Random;
            int random = rand.Next(0, 100);

            double modifiedLuckyRate = BattleCore.BaseLuckyRate * (2 - (BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth));

            if (random < modifiedLuckyRate)
            {
                Program.SetNotification("Lucky Hit!");
                return 2;
            }
            else
                return 1;
        }

        private bool CheckAccuracy(int lucky)
        {
            if (lucky == 2)
            {
                return true;
            }

            Random rand = new Random();
            int random = rand.Next(0, 100);

            double modifiedAccuracy = this.Accuracy;

            if ((BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth) < 0.5)
            {
                modifiedAccuracy = this.Accuracy * ConvertRange(0, 100, 80, 100, ((BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth) * 200)) * 0.01;
            }

            if (random < modifiedAccuracy)
                return true;
            else
            {
                Program.SetError("The attack missed!");
                return false;
            }

        }

        private bool CheckMagic()
        {
            if (BattleCore.AttackerMagic >= this.Cost)
                return true;
            else
            {
                if (BattleCore.CurrentTurn == "player")
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

        private int CheckDamage(int lucky)
        {
            Random random = Program.Random;
            double modifier = lucky * 0.6;
            double variation = (random.Next(85, 115) / 100d);

            double damage = ((BattleCore.AttackerPower * this.Power) / Math.Max(BattleCore.DefenderDefense, 1)) * modifier * variation;

            if (BattleCore.CurrentTurn == "player")
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
