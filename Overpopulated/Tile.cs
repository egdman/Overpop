using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	public enum Race
        {
            Any,
            Asian,
            Black,
            White
        };
        
        public enum Gender
        {
            Any,
            Male,
            Female
        };

        public enum Orientation
        {
            Any,
            Straight,
            Gay
        };



    public class Tile
    {
        
        public Race ERace;
        public Gender EGender;
        public Orientation EOrientation;
        public int Generation;
		public bool blocked; // blocked == true means that this tile cannot be joined
		public bool empty;



        // default constructor
		// creates empty tile with all properties set to "Any" (Generation is set to 0):
        public Tile()
        {
            ERace = Race.Any;
            EGender = Gender.Any;
            EOrientation = Orientation.Any;
            Generation = 0;
			blocked = false;
			empty = true;
        }



        // constructor with arguments
		// creates non-empty tile with specified properties:
        public Tile(Race race, Gender gender, Orientation orient, int gen)
        {
            ERace = race;
            EGender = gender;
            EOrientation = orient;
            Generation = gen;
			blocked = false;
			empty = false;
        }



		// copy constructor:
		public Tile(Tile original)
		{
			Copy(original);
		}


		public void Copy(Tile original)
		{
			ERace = original.ERace;
			EGender = original.EGender;
			EOrientation = original.EOrientation;
			Generation = original.Generation;
			blocked = original.blocked;
			empty = original.empty;
		}



		// block tile:
		public void Block()
		{
			blocked = true;
		}



		// unblock tile:
		public void Unblock()
		{
			blocked = false;
		}



		// clear Tile:
		public void ClearTile()
		{
			ERace = Race.Any;
            EGender = Gender.Any;
            EOrientation = Orientation.Any;
            Generation = 0;
			empty = true;

		}

	}
}
