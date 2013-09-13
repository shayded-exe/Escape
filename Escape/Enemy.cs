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
		public List<string> Attacks;
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
			this.Power = Stats[1];
			this.Magic = Stats[2];
			this.Attacks = Attacks;
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
			List<int> Stats,
			List<string> Attacks)
		:base(Name, Description, Stats, Attacks)
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
			List<int> Stats,
			List<string> Attacks)
		:base(Name, Description, Stats, Attacks)
		{
		}

		public override void Attack()
		{
			
		}
	}
	#endregion
}
