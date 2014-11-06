using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overpopulated
{
	// this class contains rules of compatibility and decides whether two tiles can join
	class JoinLogic
	{
		List<Rule> rules;


		//default constructor:
		public JoinLogic()
		{
			rules = new List<Rule>();
		}


		//add a rule:
		public void AddRule(Rule newRule)
		{
			rules.Add(newRule);
		}



		//check if two tiles are compatible:
		public bool IfCompatible(Tile first, Tile second)
		{
			if(!ifCompHelper(first, second)) {
				return false;
			}

			if(!ifCompHelper(second, first)) {
				return false;
			}

			return true;
		}



		//check if the main tile is compatible with the secondary tile
		bool ifCompHelper(Tile mainTile, Tile secondaryTile)
		{
			Rule finalRule = new Rule();

			foreach(var rule in rules) {

				// check if this rule is applicable to mainTile:
				if (!ifApplicable(rule, mainTile)) {
					continue;
				}
				// construct final rule:
				constructRule( finalRule, rule );	
			}

			if(!ifPropCompatible<Race>       ( mainTile.ERace,        secondaryTile.ERace,        finalRule.CompRace )        ||
			   !ifPropCompatible<Gender>     ( mainTile.EGender,      secondaryTile.EGender,      finalRule.CompGender )      ||
			   !ifPropCompatible<Orientation>( mainTile.EOrientation, secondaryTile.EOrientation, finalRule.CompOrientation ) ||
			   !ifPropCompatible<int>        ( mainTile.Generation,   secondaryTile.Generation,   finalRule.CompGeneration )) {
				return false;
			}

			return true;
		}



		//check if a rule is applicable to this tile:
		bool ifApplicable(Rule rule, Tile tile)
		{
			if (!ifAppRace        (rule.ApplicableTo.ERace,        tile.ERace)) {
				return false;
			}
			if (!ifAppGender      (rule.ApplicableTo.EGender,      tile.EGender)) {
				return false;
			}
			if (!ifAppOrientation (rule.ApplicableTo.EOrientation, tile.EOrientation)) {
				return false;
			}
			if (!ifAppGeneration  (rule.ApplicableTo.Generation,   tile.Generation)) {
				return false;
			}

			return true;
		}



		//check if rule is applicable to this tile's race:
		bool ifAppRace(Race ruleRace, Race tileRace)
		{
			if (ruleRace == Race.Any || tileRace == Race.Any) {
				return true;
			}

			if (ruleRace == tileRace) {
				return true;
			}

			return false;
		}



		//check if rule is applicable to this tile's gender:
		bool ifAppGender(Gender ruleGender, Gender tileGender)
		{
			if (ruleGender == Gender.Any || tileGender == Gender.Any) {
				return true;
			}

			if (ruleGender == tileGender) {
				return true;
			}

			return false;
		}



		//check if rule is applicable to this tile's orientation:
		bool ifAppOrientation(Orientation ruleOrientation, Orientation tileOrientation)
		{
			if (ruleOrientation == Orientation.Any || tileOrientation == Orientation.Any) {
				return true;
			}

			if (ruleOrientation == tileOrientation) {
				return true;
			}

			return false;
		}



		//check if rule is applicable to this tile's generation:
		bool ifAppGeneration(int ruleGen, int tileGen)
		{
			if (ruleGen == 0 || tileGen == 0) {
				return true;
			}

			if (ruleGen == tileGen) {
				return true;
			}

			return false;
		}



		//check property compatibility:
		bool ifPropCompatible<Property>(Property firstProperty, Property secondProperty, Rule.CompatibleWith compRule)
		{
			if (compRule == Rule.CompatibleWith.Any || compRule == Rule.CompatibleWith.NonSpecified) {
				return true;
			}

			if (compRule == Rule.CompatibleWith.Same &&
				EqualityComparer<Property>.Default.Equals(firstProperty, secondProperty)) {
				return true;
			}

			if (compRule == Rule.CompatibleWith.Other &&
				!EqualityComparer<Property>.Default.Equals(firstProperty, secondProperty)) {
				return true;
			}

			return false;
		}



		// copy properties from second second argument to first if they are specified:
		void constructRule( Rule newRule, Rule instructionRule )
		{
			if ( instructionRule.CompRace !=  Rule.CompatibleWith.NonSpecified ) {

				newRule.CompRace = instructionRule.CompRace;
			}

			if ( instructionRule.CompGender != Rule.CompatibleWith.NonSpecified ) {

				newRule.CompGender = instructionRule.CompGender;
			}

			if ( instructionRule.CompOrientation != Rule.CompatibleWith.NonSpecified ) {

				newRule.CompOrientation = instructionRule.CompOrientation;
			}

			if ( instructionRule.CompGeneration != Rule.CompatibleWith.NonSpecified ) {

				newRule.CompGeneration = instructionRule.CompGeneration;
			}
		}
	}


}
