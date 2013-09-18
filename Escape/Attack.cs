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

		private int lucky = 0;
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
			
		}
		#endregion

		#region Helper Methods
		private void CheckLucky()
		{
			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedLuckyRate = BattleCore.BaseLuckyRate * ((100 - BattleCore.AttackerHealth) * 0.0125);

			if (random < modifiedLuckyRate)
				lucky = 1;
			else
				lucky = 0;
		}

		private bool CheckAccuracy()
		{
			if (lucky == 1)
				return true;

			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedAccuracy = this.Accuracy;

			if (BattleCore.AttackerHealth < 50)
			{
				modifiedAccuracy = this.Accuracy * (BattleCore.AttackerHealth * 0.02);
			}

			if (random < modifiedAccuracy)
				return true;

			return false;
		}

		private int CalculateDamage()
		{
			double modifier = lucky * 0.5;
			double damage = ((BattleCore.AttackerPower / BattleCore.DefenderDefense) * this.Power) * modifier;

			return (int)damage;
		}
		#endregion
	}
}
