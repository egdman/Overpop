using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	class SpawnEvent
	{
		public Tile tile;
		public int i;
		public int j;


		public SpawnEvent()
		{
			i = j = 0;
			tile = new Tile();
		}



		public SpawnEvent( Tile newTile, int newI, int newJ )
		{
			Set(newTile, newI, newJ);
		}



		public void Set( Tile newTile, int newI, int newJ )
		{
			tile = newTile;
			i = newI;
			j = newJ;
		}



		public bool IsZero()
		{
			return tile.empty;
		}
	}
}
