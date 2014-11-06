using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Utils;
using Fusion.UserInterface;
using System.ComponentModel;

namespace Overpopulated
{
	class TileDrawer
	{
	
		Texture2D texture;
		string text;
		bool deleted;


		MoveToTarget moves;

		// coordinates:
		float x;
		float y;

		// target coorinates (where it wants to move)
		float targetX;
		float targetY;

		// velocity:
		float velX;
		float velY;

		// size multiplier:
		float size;

		// target size:
		float targetSize;

		// spring stiffness:
//		float stif;


		// constructor:
		public TileDrawer( float xPar, float yPar, float velXPar, float velYPar, Texture2D tx, string str )
		{
			text = str;

			texture = tx;

			x = targetX = xPar;
			y = targetY = yPar;

			velX = velXPar;
			velY = velYPar;

			size = 0.2f;
			targetSize = 1.0f;

//			stif = 150.0f;

			deleted = false;
			moves = new MoveToTarget(x, y, false);
		}



		public void AddMove( MoveToTarget mtt )
		{
			moves = mtt;
		}



		public bool Deleted()
		{
			return deleted;
		}


		public void Delete()
		{
			deleted = true;
		}

	

		// move to target:
		public void Update( GameTime gameTime, ref GameState state )
		{


				MoveToTarget mtt = moves;

				// if this tile should die after finishing the move:
				if (mtt.DieUponArrival) {
					deleted = true;
				}


				if ( Math.Abs( x - mtt.TargetX ) < 60 && Math.Abs( y - mtt.TargetY ) < 60 ) {
					velX = velY = 0;
					x = mtt.TargetX;
					y = mtt.TargetY;
					
				}
				else {

					state = GameState.RunAnimation;

					velX = Math.Round(mtt.TargetX - x) == 0 ? 0 : Math.Round(mtt.TargetX - x) > 0 ? 4000 : -4000;
					velY = Math.Round(mtt.TargetY - y) == 0 ? 0 : Math.Round(mtt.TargetY - y) > 0 ? 4000 : -4000;

				
				//	velX += ( ( mtt.TargetX - x ) * stif ) * gameTime.ElapsedSec;
				//	if ( velX * ( mtt.TargetX - x ) < 0 ) {
				//		velX -= 0.9f * velX;
				//	}
		
				//	velY += ( ( mtt.TargetY - y ) * stif ) * gameTime.ElapsedSec;
				//	if ( velY * ( mtt.TargetY - y ) < 0 ) {
				//		velY -= 0.9f * velY;
				//	}

				}
			

			x += velX * gameTime.ElapsedSec;
			y += velY * gameTime.ElapsedSec;



			if ( Math.Round(size, 1) != Math.Round(targetSize, 1) ) {

	//			state = GameState.RunAnimation;
				size += targetSize > size ? 0.2f : -0.2f;
			}
			else {
				size = targetSize;
			}
		
		}



		public float GetSize()
		{
			return size;
		}



		public Texture2D GetTexture()
		{
			return texture;
		}



		public string GetString()
		{
			return text;
		}



		public float GetX()
		{
			return x;
		}



		public float GetY()
		{
			return y;
		}

	}
}
