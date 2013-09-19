using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape
{
	class Attack : Entity
	{
		#region Declarations
		public int Power;
		public int Accuracy;
		public int Cost;
		public AttackTypes Type;

		public enum AttackTypes { Physical, Magic, Self };
		#endregion

		#region Constructor
		public Attack(
			string Name,
			string Description,
			List<int> Stats,
			AttackTypes Type)
		:base(Name, Description)
		{
			this.Power = Stats[0];
			this.Accuracy = Stats[1];
			this.Cost = Stats[2];
			this.Type = Type;
		}
		#endregion

		#region Public Methods
		public virtual void Use() 
		{
			int lucky = CheckLucky();
			bool willHit = CheckAccuracy(lucky);
			int damage = CheckDamage(lucky);

			BattleCore.DefenderHealth -= damage;
			BattleCore.AttackerMagic -= this.Cost;
		}
		#endregion

		#region Helper Methods
		private int CheckLucky()
		{
			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedLuckyRate = BattleCore.BaseLuckyRate * ((100 - BattleCore.AttackerHealth) * 0.0125);

			if (random < modifiedLuckyRate)
				return 1;
			else
				return 0;
		}

		private bool CheckAccuracy(int lucky)
		{
			if (lucky == 1)
			{
				return true;
			}

			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedAccuracy = this.Accuracy;

			if (BattleCore.AttackerHealth < 50)
			{
				modifiedAccuracy = this.Accuracy * (BattleCore.AttackerHealth * 0.02);
			}

			if (random < modifiedAccuracy)
				return true;
			else
				return false;
				
		}

		private int CheckDamage(int lucky)
		{
			double modifier = lucky * 0.5;
			double damage = ((BattleCore.AttackerPower / BattleCore.DefenderDefense) * this.Power) * modifier;

			return (int)damage;
		}
		#endregion
	}
}
