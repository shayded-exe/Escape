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
			AttackTypes Type = AttackTypes.Physical)
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
			string outputMessage = "";

			if (BattleCore.CurrentTurn == "player")
			{
				outputMessage += "You used " + this.Name;
			}
			else
			{
				outputMessage += "The enemy used " + this.Name;
			}

			if (!CheckMagic(ref outputMessage))
			{
				World.Attacks[World.GetAttackIDByName("flail")].Use();
			}
			else
			{
				BattleCore.AttackerMagic -= this.Cost;

				int lucky = CheckLucky(ref outputMessage);

				if (CheckAccuracy(lucky, ref outputMessage))
				{
					int damage = CheckDamage(lucky, ref outputMessage);

					BattleCore.DefenderHealth -= damage;
				}
			}

			Program.SetNotification(outputMessage);
		}
		#endregion

		#region Helper Methods
		private int CheckLucky(ref string outputMessage)
		{
			Random rand = new Random();
			int random = rand.Next(0, 100);

			double modifiedLuckyRate = BattleCore.BaseLuckyRate * (2 - (BattleCore.AttackerHealth / BattleCore.AttackerMaxHealth));

			if (random < modifiedLuckyRate)
			{
				outputMessage = "Lucky Hit! " + outputMessage;
				return 2;
			}
			else
				return 1;
		}

		private bool CheckAccuracy(int lucky, ref string outputMessage)
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
				outputMessage += ", but it missed!";
				return false;
			}
				
		}

		private bool CheckMagic(ref string outputMessage)
		{
			if (BattleCore.AttackerMagic >= this.Cost)
				return true;
			else
			{
				if (BattleCore.CurrentTurn == "player")
				{
					outputMessage = "Not enough magic! You flailed your arms.";
				}
				else
				{
					outputMessage = "The enemy is out of magic! It flailed its arms.";
				}
				return false;
			}
		}

		private int CheckDamage(int lucky, ref string outputMessage)
		{
			Random random = new Random();
			double modifier = lucky * 0.6;
			double variation = (random.Next(85, 115) / 100d);
			
			double damage = ((BattleCore.AttackerPower * this.Power) / Math.Max(BattleCore.DefenderDefense, 1)) * modifier * variation;

			if (BattleCore.CurrentTurn == "player")
			{
				outputMessage += " and did " + (int)damage + " damage!";
			}
			else
			{
				outputMessage += " and did " + (int)damage + " damage";
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
