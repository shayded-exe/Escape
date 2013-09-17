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
		public int Cost;
		public AttackTypes Type;

		public enum AttackTypes { Physical, Magic, Self };
		#endregion

		#region Constructor
		public Attack(
			string Name,
			string Description,
			int Power,
			int Cost,
			AttackTypes Type)
		:base(Name, Description)
		{
			this.Power = Power;
			this.Cost = Cost;
			this.Type = Type;
		}
		#endregion

		#region Public Methods
		public virtual void Use() 
		{ 
			
		}
		#endregion
	}
}
