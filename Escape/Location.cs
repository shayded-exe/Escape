using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class Location : Entity
	{
		#region Declarations
		private List<string> TempExits;
		private List<string> TempItems;
		private List<string> TempEnemies;

		public List<int> Exits;
		public List<int> Items;
		public List<int> Enemies;
		private int BattleChance;
		#endregion
		
		#region Constructors
		public Location(
			string Name,
			string Description,
			List<string> Exits,
			List<string> Items,
			List<string> Enemies,
			int BattleChance)
		:base(Name, Description)
		{
			this.TempExits = Exits;
			this.TempItems = Items;
			this.TempEnemies = Enemies;
			this.BattleChance = BattleChance;
		}
		
		public Location(
			string Name,
			string Description,
			List<string> Exits,
			List<string> Items)
		:base(Name, Description)
		{
			this.TempExits = Exits;
			this.TempItems = Items;
			this.TempEnemies = new List<string>();
			this.BattleChance = 0;
		}
		
		public Location(
			string Name,
			string Description,
			List<string> Exits)
		:base(Name, Description)
		{
			this.TempExits = Exits;
			this.TempItems = new List<string>();
			this.TempEnemies = new List<string>();
			this.BattleChance = 0;
		}
		#endregion
		
		#region Public Methods
		public bool ContainsExit(int aExit)
		{
			if (this.Exits.Contains(aExit))
				return true;
			else
				return false;
		}

		public void AddExit(int aExit)
		{
			this.Exits.Add(aExit);
		}

		public void RemoveExit(int aExit)
		{
			if (this.ContainsExit(aExit))
			{
				this.Exits.Remove(aExit);
			}
		}
		
		public bool ContainsItem(int aItem)
		{
			if (this.Items.Contains(aItem))
				return true;
			else
				return false;
		}

		public void AddItem(int aItem)
		{
			this.Items.Add(aItem);
		}

		public void RemoveItem(int aItem)
		{
			if (this.ContainsItem(aItem))
			{
				this.Items.Remove(aItem);
			}
		}
		
		public bool ContainsEnemy(int aEnemy)
		{
			if (this.Enemies.Contains(aEnemy))
				return true;
			else
				return false;
		}

		public void RemoveEnemy(int aEnemy)
		{
			if (this.ContainsEnemy(aEnemy))
			{
				this.Enemies.Remove(aEnemy);
			}
		}
		
		public bool CalculateRandomBattle()
		{
			if (Program.Rand.Next(100) < BattleChance && Enemies.Count > 0)
				return true;
			else
				return false;
		}

		public void ConvertAttributeListsToIDs()
		{
			List<int> ExitsResult = new List<int>();

			for (int i = 0; i < TempExits.Count; i++)
			{
				ExitsResult.Add(World.GetLocationIDByName(TempExits[i]));
			}
			
			this.Exits = ExitsResult;

			List<int> ItemsResult = new List<int>();

			for (int i = 0; i < TempItems.Count; i++)
			{
				ItemsResult.Add(World.GetItemIDByName(TempItems[i]));
			}

			this.Items = ItemsResult;

			List<int> EnemiesResult = new List<int>();

			for (int i = 0; i < TempEnemies.Count; i++)
			{
				EnemiesResult.Add(World.GetEnemyIDByName(TempEnemies[i]));
			}

			this.Enemies = EnemiesResult;
		}
		#endregion
	}
}