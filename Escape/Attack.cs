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
			if (!CheckMagic())
				return;

			int lucky = CheckLucky();
			
			if (!CheckAccuracy(lucky))
				return;

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

			double modifiedLuckyRate = BattleCore.BaseLuckyRate * (2 - (BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth));

			if (random < modifiedLuckyRate)
			{
				Program.SetNotification("Lucky Hit!");
				return 2;
			}
			else
				return 1;
		}

		private bool CheckAccuracy(int lucky)
		{
			if (lucky == 2)
			{
				return true;
			}

			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedAccuracy = this.Accuracy;

			if ((BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth) < 0.5)
			{
				modifiedAccuracy = this.Accuracy * ConvertRange(0, 100, 80, 100, ((BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth) * 200)) * 0.01;
			}

			if (random < modifiedAccuracy)
				return true;
			else
			{
				Program.SetError("The attack missed!");
				return false;
			}
				
		}

		private bool CheckMagic()
		{
			if (BattleCore.AttackerMagic >= this.Cost)
				return true;
			else
			{
				Program.SetError("Not enough magic to attack!");
				return false;
			}
		}

		private int CheckDamage(int lucky)
		{
			Random random = new Random();
			double modifier = lucky * 0.6;
			double variation = (random.Next(85, 115) / 100d);
			
			double damage = ((BattleCore.AttackerPower * this.Power) / Math.Max(BattleCore.DefenderDefense, 1)) * modifier * variation;

			if (BattleCore.CurrentTurn == "player")
			{
				Program.SetNotification("You did " + (int)damage + " damage!");
			}
			else
			{
				Program.SetNotification("The enemy did " + (int)damage + " damage");
			}

			return (int)damage;
		}

		public static int ConvertRange(
			int originalStart, int originalEnd,
			int newStart, int newEnd,
			double value)
		{
			double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
			return (int)(newStart + ((value - originalStart) * scale));
		}
		#endregion
	}
}
