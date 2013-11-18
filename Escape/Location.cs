using System;
using System.Collections.Generic;
using System.Linq;

namespace Escape
{
    [Serializable]
    class Location : INamed
    {
        #region Declarations
        public string Name { get; set; }
        public string Description;

        private IEnumerable<Func<Location>> unboundExits { get; set; }

        // This one is a bit questionable, but the alternative would just move the hack to Locations;
        private List<Location> exits = new List<Location>();
        public List<Location> Exits
        {
            get
            {
                if (unboundExits != null)
                {
                    foreach (var exitFunc in unboundExits)
                    { exits.Add(exitFunc()); }
                    unboundExits = null;
                }
                return exits;
            }
            set
            {
                exits = value;
                unboundExits = null;
            }
        }
        public List<Item> Items { get; private set; }
        public List<Enemy> Enemies { get; private set; }

        public int BattleChance;
        #endregion

        #region Constructor(s)
        public Location(
            string name,
            string description = "",
            IEnumerable<Func<Location>> unboundExits = null,
            IEnumerable<Item> items = null,
            IEnumerable<Enemy> enemies = null,
            int battleChance = 0)
        {
            this.Name = name;
            this.Description = description;
            this.unboundExits = unboundExits;
            this.Items = new List<Item>(items ?? Enumerable.Empty<Item>());
            this.Enemies = new List<Enemy>(enemies ?? Enumerable.Empty<Enemy>());
            this.BattleChance = battleChance;
        }
        #endregion

        #region Public Methods
        // I removed the collection wrappers, just operating on the collections shouldn't do any harm.

        public void CalculateRandomBattle(BattleCore battleCore)
        {
            // Creating new randoms on the fly yields repeat numbers. It's also slow.
            if (Program.Random.Next(100) < BattleChance && Enemies.Count > 0)
            {
                Enemy enemy = Enemies[Program.Random.Next(Enemies.Count)];
                Program.SetNotification("You were attacked by " + Text.AorAn(enemy.Name));
                battleCore.StartBattle(enemy, "enemy");
            }
        }
        #endregion
    }
}