using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Overpopulated
{

	// this class determines what kind of tile should be spawned when tiles join
	class SpawnLogic
	{
		static Random rnd = new Random();

		// default constructor:
		public SpawnLogic() {}


		// returns new Tile based on parent Tiles. If blocked == true, blocks the new tile
		// does not block if returns empty Tile
		public static Tile spawnTile( Tile parentA, Tile parentB, bool blocked )
		{

			Tile newTile = new Tile();

			if ( parentA.EGender == parentB.EGender )
			{
				newTile.ClearTile();
				return newTile;
			}

			newTile.empty = false;
	//		Random rnd = new Random();

			int race =        rnd.Next(0, 2);
			int gender =      rnd.Next(0, 2);
			int orientation = rnd.Next(0, 20);
			int generation = 1 + Math.Max( parentA.Generation, parentB.Generation );

			if ( race == 0 ) {
				newTile.ERace = parentA.ERace;
			}
			else {
				newTile.ERace = parentB.ERace;
			}

			if ( gender == 0 ) {
				newTile.EGender = Gender.Female;
			}
			else {
				newTile.EGender = Gender.Male;
			}

			if ( orientation == 0 ) {
				newTile.EOrientation = Orientation.Gay;
			}
			else {
				newTile.EOrientation = Orientation.Straight;
			}

			newTile.Generation = generation;

			if (blocked) {
				newTile.Block();
			}

			return newTile;
		}



		public static Tile SpawnRandomTile()
		{
			Tile newTile = new Tile();

			newTile.empty = false;
//			Random rnd = new Random();

			int gender =      rnd.Next(0, 2);
			int orientation = rnd.Next(0, 20);
			int race =        rnd.Next(0, 3);

			switch(race) {
				case 0:
					newTile.ERace = Race.Asian;
					break;
				case 1:
					newTile.ERace = Race.Black;
					break;
				case 2:
					newTile.ERace = Race.White;
					break;
				default:
					break;
			}


			if ( gender == 0 ) {
				newTile.EGender = Gender.Female;
			}
			else {
				newTile.EGender = Gender.Male;
			}

			if ( orientation == 0 ) {
				newTile.EOrientation = Orientation.Gay;
			}
			else {
				newTile.EOrientation = Orientation.Straight;
			}

			newTile.Generation = 1;

			return newTile;

		}



	}
}
