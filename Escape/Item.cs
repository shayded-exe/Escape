using System;
using System.Collections.Generic;

namespace Escape
{
    [Serializable]
    class Item
    {
        #region Definitions
        public delegate void OnUse(Item item);
        public delegate void OnUseInBattle(Item item, Enemy victim);
        #endregion

        #region Declarations
        public string Name;
        public string Description;

        public bool Usable;
        private event OnUse uses;
        public event OnUse Uses
        {
            add
            {
                uses += value;

                if (!Usable)
                    Usable = true;
            }
            remove
            {
                uses -= value;
            }
        }

        public bool UsableInBattle;
        private event OnUseInBattle battleUses;
        public event OnUseInBattle BattleUses
        {
            add
            {
                battleUses += value;

                if (!UsableInBattle)
                    UsableInBattle = true;
            }
            remove
            {
                battleUses -= value;
            }
        }

        // A list of arbitrary attributes indexed by a string key.
        // The usage of Hungarian Notation (the name of the variable is prefixed by its type) is advised here
        // (even though it is generally discouraged by now)
        //
        // For example: the Brass Key uses two strings, str_targetLocation and str_newLocation.
        public Dictionary<string, object> ExtendedAttributes;
        #endregion

        #region Constructor
        public Item(string name)
        {
            this.Name = name;

            // Default values
            this.Description = String.Empty;
            this.Usable = false;
            this.UsableInBattle = false;

            this.ExtendedAttributes = new Dictionary<string, object>();
        }
        #endregion

        #region Public Methods
        public void Use()
        {
            if (uses != null && Usable)
                uses(this);
            else
                NoUse();
        }

        public void UseInBattle(Enemy victim)
        {
            if (battleUses != null && UsableInBattle)
                battleUses(this, victim);
            else
                NoUse();
        }

        public void NoUse()
        {
            Program.SetError("There is a time and place for everything, but this is not the place to use that!");
        }
        #endregion
    }
}