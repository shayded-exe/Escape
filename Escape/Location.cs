using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class Location : Entity
	{
		#region Declarations
		public List<int> Exits;
		public List<int> Items;
		public List<int> Enemies;
		private int BattleChance;
		#endregion
		
		#region Constructors
		public Location(
			string Name,
			string Description,
			List<int> Exits,
			List<int> Items,
			List<int> Enemies,
			int BattleChance)
		:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = Items;
			this.Enemies = Enemies;
			this.BattleChance = BattleChance;
		}
		
		public Location(
			string Name,
			string Description,
			List<int> Exits,
			List<int> Items)
		:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = Items;
			this.Enemies = new List<int>();
			this.BattleChance = 0;
		}
		
		public Location(
			string Name,
			string Description,
			List<int> Exits)
		:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = new List<int>();
			this.Enemies = new List<int>();
			this.BattleChance = 0;
		}
		#endregion
		
		#region Public Methods
		public bool ContainsExit(int aExit)
		{
			if (Exits.Contains(aExit))
				return true;
			else
				return false;
		}
		
		public bool ContainsItem(int aItem)
		{
			if (Items.Contains(aItem))
				return true;
			else
				return false;
		}
		
		public bool ContainsEnemy(int aEnemy)
		{
			if (Enemies.Contains(aEnemy))
				return true;
			else
				return false;
		}
		
		public bool CalculateRandomBattle()
		{
			if (Program.Rand.Next(100) < BattleChance && Enemies.Count > 0)
				return true;
			else
				return false;
		}
		#endregion
	}
}