using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	class MoveToTarget
	{
		public float TargetX;
		public float TargetY;
		public bool DieUponArrival;


		public MoveToTarget( float x, float y, bool die )
		{
			TargetX = x;
			TargetY = y;
			DieUponArrival = die;
		}

	}
}
