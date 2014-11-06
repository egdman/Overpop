using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	class Rule
	{

		// this enum determines compatibility:
		public enum CompatibleWith
		{
			Any,
			Same,
			Other,
			NonSpecified
		};


		// this member determines, to which tiles this rule is applicable:
		public Tile ApplicableTo;


		//these members determine rules of compatibility:
		public CompatibleWith CompRace;
 		public CompatibleWith CompGender;
		public CompatibleWith CompOrientation;
		public CompatibleWith CompGeneration;

		

		//default constructor:
		public Rule()
		{
			ApplicableTo = new Tile();
			CompRace =        CompatibleWith.NonSpecified;
			CompGender =      CompatibleWith.NonSpecified;
			CompOrientation = CompatibleWith.NonSpecified;
			CompGeneration =  CompatibleWith.NonSpecified;
		}



		//constructor with arguments:
		public Rule(Race race, Gender gender, Orientation orient, int gen )
		{
			ApplicableTo = new Tile(race, gender, orient, gen);
			CompRace =        CompatibleWith.NonSpecified;
			CompGender =      CompatibleWith.NonSpecified;
			CompOrientation = CompatibleWith.NonSpecified;
			CompGeneration =  CompatibleWith.NonSpecified;
		}

	}

}
