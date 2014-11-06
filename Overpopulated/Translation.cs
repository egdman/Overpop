using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	class MoveEvent
	{

		public int iStart;
		public int jStart;
		public int iEnd;
		public int jEnd;
		public bool DieUponArrival;


		public MoveEvent()
		{
			iStart = 0;
			jStart = 0;
			iEnd   = 0;
			jEnd   = 0;
			DieUponArrival = false;
		}


		public MoveEvent(int iS, int jS, int iF, int jF, bool die)
		{
			iStart = iS;
			jStart = jS;
			iEnd   = iF;
			jEnd   = jF;
			DieUponArrival = die;
		}


		public bool IsZero()
		{
			return ( iStart == iEnd ) && ( jStart == jEnd );
		}
		
	}
}
