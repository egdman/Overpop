using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{


	// event of spawn without parents
	class SpawnIndep
	{
		public Tile Child;

		public int i;
		public int j;


		public SpawnIndep( Tile ch, int iNew, int jNew )
		{
			Child = ch;
			i = iNew;
			j = jNew;
		}

	
	}
}
