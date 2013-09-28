using System;
using System.Collections.Generic;

namespace Escape
{
	[Serializable]
	class Enemy : Entity
	{
		#region Declarations
		public int Health;
		public int MaxHealth;

		public int Magic;
		public int MaxMagic;

		public int Power;
		public int Defense;
		public int ExpValue;

		public List<string> TempAttacks;
		public List<int> Attacks;
		#endregion
		
		#region Constructor
		public Enemy(
			string Name,
			string Description,
			List<int> Stats,
			List<string> Attacks)
		:base(Name, Description)
		{
			this.Health = Stats[0];
			this.MaxHealth = Health;

			this.Magic = Stats[1];
			this.MaxMagic = Magic;

			this.Power = Stats[2];
			this.Defense = Stats[3];
			this.ExpValue = Stats[4];
			this.TempAttacks = Attacks;
		}
		#endregion

		#region Public Methods
		public virtual void Attack() 
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
