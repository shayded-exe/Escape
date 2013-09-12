using System;
using System.Collections.Generic;

namespace Escape
{
	abstract class Enemy : Entity
	{
		#region Declarations
		public int Health;
		public int Power;
		public int Magic;
		#endregion
		
		#region Constructor
		public Enemy(
			string Name,
			string Description,
			List<int> Stats)
		:base(Name, Description)
		{
			this.Health = Stats[0];
			this.Power = Stats[1];
			this.Magic = Stats[2];
		}
		#endregion

		#region Public Methods
		public virtual void Attack() { }
		#endregion
	}

	#region Rat
	class Rat : Enemy
	{
		public Rat(
			string Name,
			string Description,
			List<int> Stats)
		:base(Name, Description, Stats)
		{	
		}

		public override void Attack()
		{
			
		}
	}
	#endregion

	#region Hawk
	class Hawk : Enemy
	{
		public Hawk(
			string Name,
			string Description,
			List<int> Stats)
		:base(Name, Description, Stats)
		{
		}

		public override void Attack()
		{
			
		}
	}
	#endregion
}
