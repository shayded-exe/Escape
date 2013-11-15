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

        // This should behave the same way as before, except that it can't be switched dynamically.
        // You can add that with the code from the Alternative branch, and probably should add an optional parameter into the constructor.
#if !Alternative
        public bool Usable { get { return Uses != null; } }
#else
        private bool usable = true;
        public bool Usable { get { return usable && Uses != null; } set { usable = value; } }
#endif
        public event OnUse Uses;

        // Same here.
        public bool UsableInBattle { get { return BattleUses != null; } }
        public event OnUseInBattle BattleUses;

        //TODO: Get rid of this!
        // A list of arbitrary attributes indexed by a string key.
        // The usage of Hungarian Notation (the name of the variable is prefixed by its type) is advised here
        // (even though it is generally discouraged by now)
        //
        // For example: the Brass Key uses two strings, str_targetLocation and str_newLocation.
        public Dictionary<string, object> ExtendedAttributes = new Dictionary<string, object>();
        #endregion

        #region Constructor
        public Item(
            string name,
            string description = "",
            OnUse uses = null,
            OnUseInBattle battleUses = null,
            IDictionary<string, object> extendedAttributes = null)
        {
            this.Name = name;
            this.Description = description;
            this.Uses = uses;
            this.BattleUses = battleUses;

            if (extendedAttributes != null)
            {
                foreach (var kvPair in extendedAttributes)
                {
                    this.ExtendedAttributes.Add(kvPair.Key, kvPair.Value);
                }
            }
        }
        #endregion

        #region Public Methods
        public void Use()
        {
            // Usable now implies Uses != null.
            // I've added curly braces as not having them is usually discouraged.
            // This version makes it clear that commenting out the line can have side-effects.
            if (Usable)
            { Uses(this); }
            else
            { NoUse(); }
        }

        public void UseInBattle(Enemy victim)
        {
            // See above.
            if (UsableInBattle)
            { BattleUses(this, victim); }
            else
            { NoUse(); }
        }

        public void NoUse()
        {
            Program.SetError("There is a time and place for everything, but this is not the place to use that!");
        }
        #endregion
    }
}