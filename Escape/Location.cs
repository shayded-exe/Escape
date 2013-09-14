using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class Location : Entity
	{
		#region Declarations
		public List<string> Exits;
		public List<string> Items;
		public List<string> Enemies;
		private int BattleChance;
		#endregion
		
		#region Constructors
		public Location(
			string Name,
			string Description,
			List<string> Exits)
			: base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = new List<string>();
			this.Enemies = new List<string>();
		}

		public Location(
			string Name,
			string Description,
			List<string> Exits,
			List<string> Items)
			:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = Items;
			this.Enemies = new List<string>();
			this.BattleChance = 0;
		}

		public Location(
			string Name,
			string Description,
			List<string> Exits,
			List<string> Items,
			List<string> Enemies,
			int BattleChance)
		:base(Name, Description)
		{
			this.Exits = Exits;
			this.Items = Items;
			this.Enemies = Enemies;
			this.BattleChance = BattleChance;
		}
		#endregion
		
		#region Public Methods
		public void SetExits(List<string> exits)
		{
			this.Exits = exits;
		}

		public void AddExit(List<string> exits)
		{
			this.Exits.AddRange(exits);
		}

		public bool ContainsExit(string aExit)
		{
			if (Exits.Contains(aExit))
				return true;
			else
				return false;
		}
		
		public bool ContainsItem(string aItem)
		{
			if (Items.Contains(aItem))
				return true;
			else
				return false;
		}
		
		public bool ContainsEnemy(string aEnemy)
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

		private List<int> ConvertNameToID(List<String> input, string listType)
		{
			List<int> result = new List<int>();

			switch (listType)
			{
				case "location":
					for (int i = 0; i < input.Count; i++)
					{
						result.Add(World.GetLocationIdByName(input[i]));
					}
					break;
				case "item":
					for (int i = 0; i < input.Count; i++)
					{
						result.Add(World.GetItemIdByName(input[i]));
					}
					break;
				case "enemy":
					for (int i = 0; i < input.Count; i++)
					{
						result.Add(World.GetEnemyIdByName(input[i]));
					}
					break;
				default:
					result.Add(1);
					break;
			}

			return result;
		}
		#endregion
	}
}