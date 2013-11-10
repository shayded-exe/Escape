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

        private List<Attack> _Attacks;
        public List<Attack> Attacks { get { return new List<Attack>(_Attacks); } }
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
            this._Attacks = new List<Attack>();
        }
        #endregion

        #region Public Methods
        public void AddAttack(Attack attack)
        {
            if (!this.ContainsAttack(attack))
                this._Attacks.Add(attack);
        }

        public bool ContainsAttack(Attack attack)
        {
            return this._Attacks.Contains(attack);
        }

        public void RemoveAttack(Attack attack)
        {
            if (this.ContainsAttack(attack))
                this._Attacks.Remove(attack);
        }

        public void Attack()
        {
            _Attacks[new Random().Next(this._Attacks.Count)].Use();
        }
        #endregion
    }
}
