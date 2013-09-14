using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Escape
{
	class Attack
	{
		#region Declarations
		public int ID;
		public string Name;
		public int Power;
		public int Cost;
		public AttackTypes Type;

		public enum AttackTypes { Physical, Magic, Self };
		#endregion
	}
}
