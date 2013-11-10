using System;
using System.Collections.Generic;

namespace Escape
{
    [Serializable]
    class Location
    {
        #region Declarations
        public string Name;
        public string Description;

        private List<Location> _Exits;
        public List<Location> Exits { get { return new List<Location>(_Exits); } }

        private List<Item> _Items;
        public List<Item> Items { get { return new List<Item>(_Items); } }

        private List<Enemy> _Enemies;
        public List<Enemy> Enemies { get { return new List<Enemy>(_Enemies); } }

        public int BattleChance;
        #endregion

        #region Constructor(s)
        public Location(string name)
        {
            this.Name = name;

            // Default values
            this.Description = String.Empty;
            this._Exits = new List<Location>();
            this._Items = new List<Item>();
            this._Enemies = new List<Enemy>();
            this.BattleChance = 0;
        }
        #endregion

        #region Public Methods
        public void AddExit(Location exit)
        {
            if (!this.ContainsExit(exit))
                this._Exits.Add(exit);
        }

        public bool ContainsExit(Location exit)
        {
            return this._Exits.Contains(exit);
        }

        public void RemoveExit(Location exit)
        {
            if (this.ContainsExit(exit))
                this._Exits.Remove(exit);
        }

        public void AddItem(Item item)
        {
            if (!this.ContainsItem(item))
                this._Items.Add(item);
        }

        public bool ContainsItem(Item item)
        {
            return this._Items.Contains(item);
        }

        public void RemoveItem(Item item)
        {
            if (this.ContainsItem(item))
                this._Items.Remove(item);
        }

        public void AddEnemy(Enemy enemy)
        {
            if (!this.ContainsEnemy(enemy))
                this._Enemies.Add(enemy);
        }

        public bool ContainsEnemy(Enemy enemy)
        {
            return this._Enemies.Contains(enemy);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            if (this.ContainsEnemy(enemy))
                this._Enemies.Remove(enemy);
        }

        public void CalculateRandomBattle()
        {
            if (new Random().Next(100) < BattleChance && Enemies.Count > 0)
            {
                Enemy enemy = _Enemies[new Random().Next(Enemies.Count)];
                Program.SetNotification("You were attacked by " + Text.AorAn(enemy.Name));
                BattleCore.StartBattle(enemy, "enemy");
            }
        }
        #endregion
    }
}