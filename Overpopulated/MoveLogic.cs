using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	// this class moves tiles in certain direction and spawns new tiles
	class MoveLogic
	{

		JoinLogic joiner;


		//default constructor:
		public MoveLogic()
		{
			joiner = new JoinLogic();
			createRules();
		}


		
		// write rules here: ============================================================

		// (need to make rules readable from a text file later)
		void createRules()
		{
			// general rule for all people:
			Rule newRule = new Rule( Race.Any, Gender.Any, Orientation.Any, 0 );
 			newRule.CompRace =         Rule.CompatibleWith.Any;
			newRule.CompGender =       Rule.CompatibleWith.Other;
			newRule.CompOrientation =  Rule.CompatibleWith.Same;
			newRule.CompGeneration =   Rule.CompatibleWith.Same;
			joiner.AddRule(newRule);

			// rule overload for gay people:
			newRule = new Rule( Race.Any, Gender.Any, Orientation.Gay, 0 );
			newRule.CompGender =       Rule.CompatibleWith.Same;
			joiner.AddRule(newRule);

			// rule overload for first generation:
			newRule = new Rule( Race.Any, Gender.Any, Orientation.Straight, 1 );
 			newRule.CompRace =         Rule.CompatibleWith.Same;
			joiner.AddRule(newRule);

			// rule overload for second generation:
		//  newRule = new Rule( Race.Any, Gender.Any, Orientation.Straight, 2);
 		//	newRule.CompRace =         Rule.CompatibleWith.Same;
		//	joiner.AddRule(newRule);

		}
		// ==============================================================================


		public int SweepTiles( Grid grid, Direction dir, List<MoveEvent> translations, List<SpawnEvent> spawns )
		{
			int size = grid.GetSize();
			int scoreGain = 0;
			bool tilesMoved = false;

			switch (dir) {

				case Direction.Up:
					for     (int j = 0; j < size; ++j) {
						for (int i = 1; i < size; ++i) {

							if ( moveTile(grid, i, j, dir, translations, spawns, ref scoreGain ) ) {
								tilesMoved = true;
							}

						}
					}
					break;

				case Direction.Right:
					for     (int i = 0; i < size; ++i) {
						for (int j = size - 2; j >= 0; --j) {

							if ( moveTile(grid, i, j, dir, translations, spawns, ref scoreGain ) ) {
								tilesMoved = true;
							}

						}
					}
					break;

				case Direction.Down:
					for     (int j = 0; j < size; ++j) {
						for (int i = size - 2; i >=0; --i) {

							if ( moveTile(grid, i, j, dir, translations, spawns, ref scoreGain ) ) {
								tilesMoved = true;
							}

						}
					}
					break;

				case Direction.Left:
					for     (int i = 0; i < size; ++i) {
						for (int j = 1; j < size; ++j) {

							if ( moveTile(grid, i, j, dir, translations, spawns, ref scoreGain ) ) {
								tilesMoved = true;
							}

						}
					}
					break;

				default:
					break;

			}
			// Unblock all the newly created tiles:
			grid.UnblockAll();


			// spawn a random tile if at least one tile has been moved:
			if (tilesMoved) {
				grid.SpawnRandow();
				Tile newTile = new Tile();
				int iNew = 0;
				int jNew = 0;
				if ( grid.GetLastSpawned(ref iNew, ref jNew, newTile) ) {
					SpawnEvent sp = new SpawnEvent(newTile, iNew, jNew);
					spawns.Add(sp);
				}
			}

			return scoreGain;
		}



		// this function moves a tile at [i, j] in specified direction and adds move and spawn events
		bool moveTile( Grid grid, int i, int j, Direction dir, List<MoveEvent> translations,
			List<SpawnEvent> spawns, ref int scoreGain )
		{
			bool tilesMoved = false;
		
			tilesMoved = moveTileWhileCan(grid, i, j, dir, translations, spawns, ref scoreGain);
			
			return tilesMoved;
		}
		



		// move tile at [i,j] all the way in specified direction
		bool moveTileWhileCan( Grid grid, int i, int j, Direction dir,
			List<MoveEvent> mEvList, List<SpawnEvent> spEvList, ref int scoreGain )
		{
			bool ifMoved = false;

			Tile thisTile = new Tile();
			if ( !grid.GetTile(i, j, ref thisTile) ) {
				return false;
			}
			if (thisTile.empty) {
				return false;
			}

			MoveEvent mEv = new MoveEvent();
			mEv.DieUponArrival = false;

			int iNext    = i;
			int jNext    = j;

			mEv.iStart   = i;
			mEv.jStart   = j;

			mEv.iEnd     = i;
			mEv.jEnd     = j;

			moveCoords( ref iNext, ref jNext, dir ); // shift next coords 1 step in dir

			Tile nextTile = new Tile();

			while ( grid.GetTile( iNext, jNext, ref nextTile ) ) {

				if (nextTile.empty) { 
					ifMoved = true;

					mEv.iEnd = iNext;
					mEv.jEnd = jNext;

					moveCoords(ref iNext, ref jNext, dir );
				}
				else if ( !nextTile.blocked && joiner.IfCompatible( thisTile, nextTile ) ) {

					// add score:
					scoreGain += Math.Max( thisTile.Generation, nextTile.Generation );

					
					// spawn new tile at [iNext, jNext] using spawn logic:
					Tile newTile = SpawnLogic.spawnTile( thisTile, nextTile, true );

					grid.OverwriteTile( newTile, iNext, jNext );
					grid.ClearTile( i, j );

					mEv.iEnd = iNext;
					mEv.jEnd = jNext;
					mEv.DieUponArrival = true;

					MoveEvent mEv2 = new MoveEvent();
					mEv2.iStart = mEv2.iEnd = iNext;
					mEv2.jStart = mEv2.jEnd = jNext;
					mEv2.DieUponArrival = true;

					if (!mEv2.IsZero()) {
						mEvList.Add(mEv2);
					}

					SpawnEvent spEv = new SpawnEvent(new Tile(newTile), iNext, jNext);
					if (!spEv.IsZero()) {
						spEvList.Add(spEv);
					}

					ifMoved = true;
					break;
				}
				else {
					break;
				}	
			}

			if (!mEv.IsZero()) {

				mEvList.Add(mEv);

				if (!thisTile.empty) {
					grid.MoveTile(i, j, mEv.iEnd, mEv.jEnd);
				}
			}
			return ifMoved;
		}



		// move coordinates 1 step in specified direction
		void moveCoords( ref int iNew, ref int jNew, Direction dir )
		{
			switch (dir) {
				case Direction.Up:
					--iNew;
					break;
				case Direction.Right:
					++jNew;
					break;
				case Direction.Down:
					++iNew;
					break;
				case Direction.Left:
					--jNew;
					break;
				default:
					break;
			}

		}



		public void SpawnRandom( Grid grid, List<SpawnEvent> spawns )
		{
			grid.SpawnRandow();

			Tile spawnedTile = new Tile();
			int iSP = -1;
			int jSp = -1;

			if ( grid.GetLastSpawned(ref iSP, ref jSp, spawnedTile) ) {
				spawns.Add( new SpawnEvent(new Tile(spawnedTile), iSP, jSp) );
			}
		}



		// spawn tile at [i, j]:
		public void DebugSpawn( Grid grid, Tile tile, int i, int j, List<SpawnEvent> spawns )
		{
			grid.AddTile(tile, i, j);
			spawns.Add(new SpawnEvent(tile, i, j));
		}


	}
}
