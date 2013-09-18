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

		private bool isLucky;
		#endregion

		#region Constructor
		public Attack(
			string Name,
			string Description,
			int Power,
			int Accuracy,
			int Cost,
			AttackTypes Type)
		:base(Name, Description)
		{
			this.Power = Power;
			this.Accuracy = Accuracy;
			this.Cost = Cost;
			this.Type = Type;
		}
		#endregion

		#region Public Methods
		public virtual void Use() 
		{
			
		}
		#endregion

		#region Helper Methods
		private bool checkLucky()
		{
			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedLuckyRate = BattleCore.BaseLuckyRate * ((100 - BattleCore.CurrentEnemy.Health) * 0.0125);

			if (random < modifiedLuckyRate)
				return true;

			return false;
		}

		private bool checkAccuracy()
		{
			if (isLucky)
				return true;

			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedAccuracy = this.Accuracy;

			if (BattleCore.CurrentEnemy.Health < 50)
			{
				modifiedAccuracy = this.Accuracy * (BattleCore.CurrentEnemy.Health * 0.02);
			}

			if (random < modifiedAccuracy)
				return true;

			return false;
		}
		#endregion
	}
}
