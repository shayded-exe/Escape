using System;
using System.Collections.Generic;

namespace Escape
{
    [Serializable]
    class Enemy
    {
        #region Declarations
        public string Name;
        public string Description;

        public int Health;
        public int MaxHealth;

        public int Magic;
        public int MaxMagic;

        public int Power;
        public int Defense;
        public int ExpValue;

        // TODO: Revise this madness.
        public List<string> TempAttacks;
        public List<int> Attacks;
        #endregion

        #region Properties
        public int ID
        {
            get
            {
                return World.GetEnemyIDByName(this.Name);
            }
        }
        #endregion

        #region Constructor
        public Enemy(string name)
        {
            this.Name = name;

            // Defaults
            // TODO: Handling health and mana max. without need for double lines outside.
            this.Description = String.Empty;
            this.Health = 0;
            this.MaxHealth = 0;
            this.Magic = 0;
            this.MaxMagic = 0;
            this.Power = 0;
            this.Defense = 0;
            this.ExpValue = 0;
            this.TempAttacks = new List<string>();
            this.Attacks = new List<int>();
        }
        #endregion

        #region Public Methods
        public void Attack()
        {
            Random rand = new Random();
            int attackToUse = rand.Next(0, this.Attacks.Count);

            World.Attacks[Attacks[attackToUse]].Use();
        }

        public void ConvertAttributeListsToIDs()
        {
            List<int> AttacksResult = new List<int>();

            for (int i = 0; i < TempAttacks.Count; i++)
            {
                AttacksResult.Add(World.GetAttackIDByName(TempAttacks[i]));
            }

            this.Attacks = AttacksResult;
        }
        #endregion
    }
}
