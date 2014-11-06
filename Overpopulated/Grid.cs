using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{

	// this enum determines direction:
	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}



	struct IntPair
	{
		public int First;
		public int Second;

		// constructor:
		public IntPair(int first, int second)
		{
			First = first;
			Second = second;
		}

	}



	// Grid class contains all the tiles in game and allows to change their properties and move them
	class Grid
	{
		// 2d array of tiles:
		Tile[,] tiles;

		// grid size:
		int gridSize;

		// List of empty grid cells:
		List<IntPair> emptyCells;


		int iLastSpawned;
		int jLastSpawned;

		Random rnd;

		// constructor
		// initializes all tiles as empty:
		public Grid(int size)
		{
			rnd = new Random();
			tiles = new Tile[size,size];
			gridSize = size;

			emptyCells = new List<IntPair> {};

			Clear();
		}


		// clear grid:
		public void Clear()
		{
			fillGrid();
			fillEmptyList();

			iLastSpawned = -1;
			jLastSpawned = -1;

		}


		// fill grid with empty tiles:
		void fillGrid()
		{
			for (int i = 0; i < gridSize; ++i) {
				for (int j = 0; j < gridSize; ++j)
				{
					tiles[i, j] = new Tile();
				}
			}
		}



		// returns the size of the grid:
		public int GetSize()
		{
			return gridSize;
		}



		// if Tile object at [i,j] is empty, overwrite it with newTile
		public bool AddTile( Tile newTile, int i, int j )
		{

			if ( InBounds(i,j) ) {

				if ( tiles[i,j].empty ) {
					tiles[i,j] = newTile;

					if (!newTile.empty) {
						deleteFromEmptyList(i, j);
					}

					return true;
				}
				else {
					return false;
				}
			}
			else {
				return false;
			}

		}



		// overwrite tile at [i,j] with newTile
		public bool OverwriteTile( Tile newTile, int i, int j )
		{

			if ( InBounds(i,j) ) {

				if ( tiles[i,j].empty && !newTile.empty ) {
					deleteFromEmptyList(i, j);
				}
				else if ( !tiles[i, j].empty && newTile.empty ) {
					addToEmptyList(i, j);
				}

				tiles[i,j] = newTile;
				return true;
			}
			else {
				return false;
			}

		}



		// mark Tile at [i,j] empty:
		public bool ClearTile( int i, int j )
		{

			if ( InBounds(i,j) ) {
				tiles[i,j].ClearTile();
				addToEmptyList(i, j);
				return true;
			}
			else {
				return false;
			}

		}



		// get Tile at [i,j]:
		public bool GetTile( int i, int j, ref Tile outTile )
		{

			if ( InBounds(i,j) ) {
				outTile = tiles[i,j];
				return true;
			}
			else {
				return false;
			}

		}



		// check if [i,j] is in bounds of the grid:
		public bool InBounds( int i, int j )
		{

			if ( i >= 0 && i < gridSize && j >= 0 && j < gridSize ) {
				return true;
			}
			else {
				return false;
			}

		}



		// move tile from [i1,j1] to [i2,j2]
		public bool MoveTile( int i1, int j1, int i2, int j2 )
		{
			if (!InBounds(i1, j1) || !InBounds(i2, j2)) {
				return false;
			}

			OverwriteTile( new Tile(tiles[i1, j1]), i2, j2 );
			ClearTile(i1, j1);
			return true;
		}



		// shift tile at [i,j] 1 step in specified direction:
		public bool ShiftTile( int i, int j, Direction dir )
		{
			if ( !InBounds(i, j) ) {
				return false;
			}

			switch(dir) {

				case Direction.Up:
					return MoveTile(i, j,   i - 1, j    );

				case Direction.Right:
					return MoveTile(i, j,   i,     j + 1);

				case Direction.Down:
					return MoveTile(i, j,   i + 1, j    );
					
				case Direction.Left:
					return MoveTile(i, j,   i,     j - 1);

				default:
					return false;
			}

		}



		// METHODS TO HANDLE LIST OF EMPTY CELLS:

		// this method adds all cells to list of empty cells: 
		void fillEmptyList()
		{
			emptyCells.Clear();
			for ( int i = 0; i < gridSize; ++i ) {
				for ( int j = 0; j < gridSize; ++j ) {
					emptyCells.Add(new IntPair(i,j));
				}

			}
			
		}



		// delete specified cell from empty cells list
		bool deleteFromEmptyList( int i, int j )
		{
			if (!InBounds(i,j)) {
				return false;
			}
			int indexToDelete = -1;
			indexToDelete = emptyCells.FindIndex( pair => i == pair.First && j == pair.Second );

			// if cell not found:
			if ( indexToDelete == -1 ) {
				return false;
			}

			emptyCells.RemoveAt( indexToDelete );
			return true;

		}




		// add specified cell to empty cells list
		bool addToEmptyList( int i, int j )
		{
			if (!InBounds(i,j)) {
				return false;
			}
			int itemExistsAt = -1;
			itemExistsAt = emptyCells.FindIndex( pair => i == pair.First && j == pair.Second );

			// if this cell is already on the list:
			if ( itemExistsAt != -1 ) {
				return false;
			}

			emptyCells.Add( new IntPair(i,j) );
			return true;
		}



		// return number of empty cells:
		public int EmptyCellsCount()
		{
			return emptyCells.Count;
		}



		// unblock all tiles in the grid:
		public void UnblockAll()
		{
			for ( int i = 0; i < gridSize; ++i ) {
				for ( int j = 0; j < gridSize; ++j ) {

					tiles[i, j].Unblock();

				}
			}
		}


		// spawn random tile at random place:
		public void SpawnRandow()
		{
			if (EmptyCellsCount() == 0) {
				return;
			}
//			Random rnd = new Random();
			int index = rnd.Next( 0, EmptyCellsCount() );

			IntPair coords = emptyCells[index];
			AddTile(SpawnLogic.SpawnRandomTile(), coords.First, coords.Second);

			iLastSpawned = coords.First;
			jLastSpawned = coords.Second;

		}



		// get last randomly spawned tile and its coordinates:
		public bool GetLastSpawned( ref int i, ref int j, Tile tile )
		{

			if (!InBounds(iLastSpawned, jLastSpawned)) {
				return false;
			}
			tile.Copy( tiles[iLastSpawned, jLastSpawned] );
			i = iLastSpawned;
			j = jLastSpawned;
			return true;

		}



		public List<IntPair> GetEmptyCells()
		{
			List<IntPair> list = new List<IntPair>();
			for (int i = 0; i < emptyCells.Count; ++i) {
				list.Add(emptyCells[i]);
			}
			return list;
		}


	}
}
