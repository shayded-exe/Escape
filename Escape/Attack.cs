using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape
{
	class Attack
	{
		#region Declarations
		private int Power;
		private int Cost;
		private AttackTypes Type;

		public enum AttackTypes { Physical, Magic, Self };
		#endregion

		#region Constructor

		#endregion

		#region Public Methods
		public virtual void Use() { }
		#endregion
	}
}
