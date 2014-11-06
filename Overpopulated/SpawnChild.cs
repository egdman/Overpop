using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	class SpawnChild
	{

		public TileDrawer ParentA;
		public TileDrawer ParentB;
		public Tile Child;

		public int i;
		public int j;


		public SpawnChild(TileDrawer pA, TileDrawer pB, Tile ch, int iNew, int jNew)
		{
			ParentA = pA;
			ParentB = pB;
			Child = ch;
			i = iNew;
			j = jNew;
		}


	}
}
