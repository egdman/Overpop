//#define DEV

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


		public enum GameState
		{
			WaitUserInput,
			RunAnimation
		}


		public enum ButtonPressed
		{
			Up,
			Right,
			Down,
			Left,
			None
		}


		public class GameConfig
		{
			int hiScore;
			[Category("Highest Score")]
			[Description("This is the highest score achieved ever")]
			public int HighScore { get { return hiScore; } set { hiScore = value; } }

			public GameConfig()
			{
				HighScore = 0;
			}
		}



		public class GridDrawer : GameService
		{
			[Config]
			public GameConfig GameCfg {get; set;}

			Grid grid;
			MoveLogic mover;
			SpriteFont font;

			Texture2D ch_m;
			Texture2D ch_f;
			Texture2D bl_m;
			Texture2D bl_f;
			Texture2D wh_m;
			Texture2D wh_f;

			Texture2D ch_m_g;
			Texture2D ch_f_g;
			Texture2D bl_m_g;
			Texture2D bl_f_g;
			Texture2D wh_m_g;
			Texture2D wh_f_g;

			Texture2D blank;

			Color backColor;

			Frame border;

			Frame scoreBoard;
			Frame scoreBoardTitle;

			Frame hiScoreBoard;
			Frame hiScoreBoardTitle;

			Frame restartBtn;

			Queue<TileDrawer>[,] tileDrawers;

			List<SpawnChild> spawnEvents; // list of tile-join-spawn events
			List<SpawnIndep> indSpawnEvents; // list of independent spawn events (without joining tiles)

			Queue<ButtonPressed> buttonEvents;

			float x;
			float y;
			int tileSize;
			int tilePadding;

			int currentScore;

			GameState state;

			int gridSize;



			// constructor:
			public GridDrawer( Game game ): base(game)
			{

				GameCfg = new GameConfig();

				GameCfg.HighScore = 0;

                // Grid size ===========================================
				gridSize = 6;
                // =====================================================


				grid =  new Grid( gridSize );

				state = GameState.WaitUserInput;

				mover = new MoveLogic();

				spawnEvents    = new List<SpawnChild>();
				indSpawnEvents = new List<SpawnIndep>();

				buttonEvents   = new Queue<ButtonPressed>(){};

				// coordinates of a grid:
				x = 40;
				y = 40;
				// background color:
				backColor = Color.Black;

				
				tileSize    = (int)Math.Round( (double)480 / gridSize );
				tilePadding = (int)Math.Round( (double)tileSize * 0.1f );

				currentScore = 0;


				// initialize tileDrawer queues:
				tileDrawers = new Queue<TileDrawer>[gridSize, gridSize];
				for ( int i = 0; i < gridSize; ++i ) {
					for ( int j = 0; j < gridSize; ++j ) {
						tileDrawers[i, j] = new Queue<TileDrawer>(){};
					}
				}

			}




			// Initialize object
			public override void Initialize()
			{
				base.Initialize();

				font = Game.Content.Load<SpriteFont>("stencil");

				ch_m   = Game.Content.Load<Texture2D>("ch_boy");
				ch_f   = Game.Content.Load<Texture2D>("ch_girl");
				ch_m_g = Game.Content.Load<Texture2D>("ch_boy_r");
				ch_f_g = Game.Content.Load<Texture2D>("ch_girl_r");

				bl_m   = Game.Content.Load<Texture2D>("bl_boy");
				bl_f   = Game.Content.Load<Texture2D>("bl_girl");
				bl_m_g = Game.Content.Load<Texture2D>("bl_boy_r");
				bl_f_g = Game.Content.Load<Texture2D>("bl_girl_r");
		
				wh_m   = Game.Content.Load<Texture2D>("wh_boy");
				wh_f   = Game.Content.Load<Texture2D>("wh_girl");
				wh_m_g = Game.Content.Load<Texture2D>("wh_boy_r");
				wh_f_g = Game.Content.Load<Texture2D>("wh_girl_r");

				blank  = Game.Content.Load<Texture2D>("blank");

				Game.InputDevice.KeyDown += InputDevice_KeyDown;

				scoreBoardTitle = new Frame(Game,
									   (int)(x + 550),
									   (int)y - 20,
									   200, 
									   50,
									   "Score:",
									   Color.Zero)
				{
					Font = font,
					TextAlignment = Alignment.MiddleCenter
				};


				scoreBoard = new Frame(Game,
									   (int)(x + 550),
									   (int)y + 30,
									   200, 
									   50,
									   "",
									   Color.Zero)
				{
					Font = font,
					Border = 1,
					BorderColor = Color.Red,
					TextAlignment = Alignment.MiddleCenter
				};

				
				hiScoreBoardTitle = new Frame(Game,
									   (int)(x + 550),
									   (int)(y + 110),
									   200, 
									   50,
									   "Hi Score:",
									   Color.Zero)
				{
					Font = font,
					TextAlignment = Alignment.MiddleCenter
				};



				hiScoreBoard = new Frame(Game,
									   (int)(x + 550),
									   (int)(y + 160),
									   200, 
									   50,
									   "",
									   Color.Zero)
				{
					Font = font,
					Border = 1,
					BorderColor = Color.Red,
					TextAlignment = Alignment.MiddleCenter
				};





				border = new Frame(Game, 
								  (int)x, // coord x
								  (int)y, // coord y
					              tilePadding + (tileSize + tilePadding) * gridSize, // width
								  tilePadding + (tileSize + tilePadding) * gridSize, // height
								  "",
								  Color.Zero)
				{
					Border = 1,
					BorderColor = Color.White
				};


				restartBtn = new Frame( Game,
										(int)x + 550,
										(int)y + 485,
										200,
										50,
										"RESTART",
										Color.Zero )
				{
					Font = font,
					Border = 1,
					BorderColor = Color.Red,
					TextAlignment = Alignment.MiddleCenter
				};

				restartBtn.StatusChanged += (s, e) =>
				{
					if ( e.Status == FrameStatus.Hovered ) {
 						restartBtn.Border = 2;
					}
					if ( e.Status == FrameStatus.None ) {
						restartBtn.Border = 1;
					}
				};


				restartBtn.Click += (s, e) => {
					startGame();
				};

				startGame();
				
			}



			public void InitUI(Frame rootFrame)
			{
				rootFrame.Add(scoreBoard);
				rootFrame.Add(scoreBoardTitle);
				rootFrame.Add(hiScoreBoardTitle);
				rootFrame.Add(hiScoreBoard);
				rootFrame.Add(border);
				rootFrame.Add(restartBtn);
			}



			void updateUI()
			{
				scoreBoard.Text = currentScore.ToString();
				hiScoreBoard.Text = GameCfg.HighScore.ToString();
			}



			// key press detector:
			void InputDevice_KeyDown(object sender, InputDevice.KeyEventArgs e)
			{
				if (e.Key == Keys.Up) {
					Log.LogInfo("UP!!!");
					buttonEvents.Enqueue(ButtonPressed.Up);
				}
				if (e.Key == Keys.Right) {
					Log.LogInfo("RIGHT!!!");
					buttonEvents.Enqueue(ButtonPressed.Right);
					
				}
				if (e.Key == Keys.Down) {
					Log.LogInfo("DOWN!!!");
					buttonEvents.Enqueue(ButtonPressed.Down);
					
				}
				if (e.Key == Keys.Left) {
					Log.LogInfo("LEFT!!!");
					buttonEvents.Enqueue(ButtonPressed.Left);
					
				}
				if (e.Key == Keys.Space) {
					printEmptyCells();
				}
				if (e.Key == Keys.Q) {
					printTileDrawers();
				}
			}



			// this function clears the tiles and spawns two random ones:
			void startGame()
			{
				currentScore = 0;

				grid.Clear();
				clearAll();

				List<SpawnEvent> spawns = new List<SpawnEvent>(){};
				// Spawn for debug: ===================================

	//			mover.DebugSpawn(grid, new Tile(Race.White, Gender.Female, Orientation.Gay, 2), 0, 0, spawns);
	//			mover.DebugSpawn(grid, new Tile(Race.White, Gender.Male,   Orientation.Gay, 3), 0, 2, spawns);
	//			mover.DebugSpawn(grid, new Tile(Race.White, Gender.Male,   Orientation.Straight, 1), 0, 3, spawns);

				// ====================================================


				// Spawn random tiles: ================================
				

				mover.SpawnRandom(grid, spawns);
				mover.SpawnRandom(grid, spawns);

				// ====================================================

				foreach( var sp in spawns ) {
					enqueueSpawn(sp);
				}
			}



			// Update object:
			public override void Update( GameTime gameTime )
			{

				if (currentScore > GameCfg.HighScore) {
						GameCfg.HighScore = currentScore;
					}
				updateUI();

				state = GameState.WaitUserInput;

				// update animations (animations should be handled before spawn events!):
				updateTileDrawers(gameTime);

				// track spawn events:
				trackSpawnEvents();

				// handle random spawns:
				trackIndepSpawns();

				// User input handling:
				if (state == GameState.WaitUserInput) {

					// remove all deleted tile drawers:
					cleanUpTiles();

					if (buttonEvents.Count != 0) {
						ButtonPressed btnPress = ButtonPressed.None;
						btnPress = buttonEvents.Dequeue();


						List<MoveEvent> translations = new List<MoveEvent>(){};
						List<SpawnEvent> spawns = new List<SpawnEvent>(){};

						switch(btnPress) {
							case ButtonPressed.Up:
								currentScore += mover.SweepTiles(grid, Direction.Up,    translations, spawns);
								break;

							case ButtonPressed.Right:
								currentScore += mover.SweepTiles(grid, Direction.Right, translations, spawns);
								break;

							case ButtonPressed.Down:
								currentScore += mover.SweepTiles(grid, Direction.Down,  translations, spawns);
								break;

							case ButtonPressed.Left:
								currentScore += mover.SweepTiles(grid, Direction.Left,  translations, spawns);
								break;

							case ButtonPressed.None:
								break;

							default:
								break;
						}

						Log.LogInfo(currentScore.ToString());

						foreach(var tr in translations) {

							enqueueMove(tr);
						}

						foreach(var sp in spawns) {

							enqueueSpawn(sp);

						}
	
					}
				}

				base.Update(gameTime);
			}



			// Draw object:
			public override void Draw(GameTime gameTime, StereoEye stereoEye)
			{
				
				var sb = Game.GetService<SpriteBatch>();

				BlendState bs = new BlendState();
	//			bs.SrcAlpha = Blend.SrcAlpha;
	//			

				sb.Begin(bs);
				
				// draw background:
				sb.DrawSprite(
					sb.TextureWhite,
					x + ( tilePadding + (tileSize + tilePadding) * gridSize ) / 2, // coord x
					y + ( tilePadding + (tileSize + tilePadding) * gridSize ) / 2, // coord y
					tilePadding + (tileSize + tilePadding) * gridSize, // width
					tilePadding + (tileSize + tilePadding) * gridSize, // height
					0,  // angle
					backColor
				);

				// draw tile texture:
				for (int i = 0; i < gridSize; ++i) {
					for (int j = 0; j < gridSize; ++j) {
						Queue<TileDrawer> tileQ = tileDrawers[i, j];

#if DEV
						Tile currentTile = new Tile();
						grid.GetTile(i, j, ref currentTile);

						sb.DrawSprite(
							getTileTexture(currentTile),
							pixelCoord(i, j).First  + 15,
							pixelCoord(i, j).Second + 15,
							tileSize, 
							tileSize,
							0,
							Color.White );
#endif
						foreach( var td in tileQ ) {
							sb.DrawSprite(
								td.GetTexture(),
								td.GetX(),
								td.GetY(),
								tileSize * td.GetSize(),
								tileSize * td.GetSize(),
								0,
								Color.White
							);
						}
					}
				}
				sb.End();

				sb.Begin();
				// draw tile text:
				for (int i = 0; i < gridSize; ++i) {
					for (int j = 0; j < gridSize; ++j) {
						Queue<TileDrawer> tileQ = tileDrawers[i, j];

						foreach( var td in tileQ ) {
							font.DrawString(
								sb,
								td.GetString(),
								td.GetX() + 0.14f * tileSize * td.GetSize(), 
								td.GetY() + 0.4f * tileSize * td.GetSize(),
								Color.Red 
							);
						}
					}
				}

				sb.End();

				base.Draw(gameTime, stereoEye);
			}


			
			// get texture for a particular type of a tile:
			Texture2D getTileTexture( Tile tile )
			{
				if (tile.empty) {
					return blank;
				}

				if (tile.ERace == Race.Asian && tile.EGender == Gender.Female) {
					if (tile.EOrientation == Orientation.Gay) {
						return ch_f_g;
					}
					else {
						return ch_f;
					}
				}

				if (tile.ERace == Race.Asian && tile.EGender == Gender.Male) {
					if (tile.EOrientation == Orientation.Gay) {
						return ch_m_g;
					}
					else {
						return ch_m;
					}
				}

				if (tile.ERace == Race.Black && tile.EGender == Gender.Female) {
					if (tile.EOrientation == Orientation.Gay) {
						return bl_f_g;
					}
					else {
						return bl_f;
					}
				}

				if (tile.ERace == Race.Black && tile.EGender == Gender.Male) {
					if (tile.EOrientation == Orientation.Gay) {
						return bl_m_g;
					}
					else {
						return bl_m;
					}
				}

				if (tile.ERace == Race.White && tile.EGender == Gender.Female) {
					if (tile.EOrientation == Orientation.Gay) {
						return wh_f_g;
					}
					else {
						return wh_f;
					}
				}

				if (tile.ERace == Race.White && tile.EGender == Gender.Male) {
					if (tile.EOrientation == Orientation.Gay) {
						return wh_m_g;
					}
					else {
						return wh_m;
					}
				}
				return blank;
			}



			// dispatch target coordinates to all tile drawers:
			void enqueueMove( MoveEvent me )
			{
				MoveToTarget mtt = new MoveToTarget( 
					pixelCoord(me.iEnd, me.jEnd).First,
					pixelCoord(me.iEnd, me.jEnd).Second,
					me.DieUponArrival );

				Queue<TileDrawer> tileQ = tileDrawers[me.iStart, me.jStart];
				
				if ( tileQ != null && tileQ.Count != 0 ) {

					TileDrawer tDrawer = tileQ.Dequeue();
					tDrawer.AddMove(mtt);
					tileDrawers[me.iEnd, me.jEnd].Enqueue(tDrawer);
				}
				
			}



			// enqueue new spawn events:
			void enqueueSpawn( SpawnEvent se )
			{
				Queue<TileDrawer> tileQ = tileDrawers[se.i, se.j];

				// if spawn is due to joined tiles:
				if ( tileQ != null && tileQ.Count >= 2 ) {
					SpawnChild newSpawnChild = new SpawnChild(tileQ.ElementAt(0), tileQ.ElementAt(1), se.tile, se.i, se.j);
					spawnEvents.Add(newSpawnChild);
					
				}

				// if spawn is independent:
				else {
					SpawnIndep spInd = new SpawnIndep( se.tile, se.i, se.j );
					indSpawnEvents.Add( spInd );
					
				}
	
			}


			
			// update states of all tile drawers:
			void updateTileDrawers( GameTime time )
			{
				for ( int i = 0; i < gridSize; ++i ) {
					for ( int j = 0; j < gridSize; ++j ) {

						Queue<TileDrawer> tileQ = tileDrawers[i, j];
						if (tileQ != null) {
							for( int k = 0; k < tileQ.Count; ++k ) {
								tileQ.ElementAt(k).Update(time, ref state);
							}
						}

					}
				}
			}



			// create child tile drawers when parent tile drawers come close to each other:
			void trackSpawnEvents()
			{
				for (int i = 0; i < spawnEvents.Count; ++i) {
					SpawnChild sc = spawnEvents.ElementAt(i);

					if (sc.ParentA != null && sc.ParentB != null ) {

						if ( ( sc.ParentA.GetX() - sc.ParentB.GetX() ) * ( sc.ParentA.GetX() - sc.ParentB.GetX() ) +
							 ( sc.ParentA.GetY() - sc.ParentB.GetY() ) * ( sc.ParentA.GetY() - sc.ParentB.GetY() ) <=
							10) {

							// creating new TileDrawer:
							TileDrawer newTD = new TileDrawer( 
								pixelCoord(sc.i, sc.j).First,
								pixelCoord(sc.i, sc.j).Second,
								0,
								0,
								getTileTexture(sc.Child),
								sc.Child.Generation.ToString() );

							tileDrawers[ sc.i, sc.j ].Enqueue(newTD);

							// delete parents:
							sc.ParentA.Delete();
							sc.ParentB.Delete();

							spawnEvents.RemoveAt(i);
						}
					}
				}
			}



			// create new tile drawers without parents:
			void trackIndepSpawns()
			{
				for ( int i = 0; i < indSpawnEvents.Count; ++i ) {
					SpawnIndep spInd = indSpawnEvents.ElementAt(i);

					TileDrawer newTD = new TileDrawer(
						pixelCoord(spInd.i, spInd.j).First,
						pixelCoord(spInd.i, spInd.j).Second,
						0,
						0,
						getTileTexture(spInd.Child),
						spInd.Child.Generation.ToString() );


					tileDrawers[spInd.i, spInd.j].Enqueue(newTD);

					indSpawnEvents.RemoveAt(i);
				}
			}



			// remove deleted tile drawers:
			void cleanUpTiles()
			{
				for (int i = 0; i < gridSize; ++i ) {
					for (int j = 0; j < gridSize; ++j ) {


						Queue<TileDrawer> tileQ = tileDrawers[i, j];
						if (tileQ.Count != 0) {
							Queue<TileDrawer> bufferQ = new Queue<TileDrawer>(){};
							foreach(var td in tileQ) {
								if (!td.Deleted()) {
									bufferQ.Enqueue(td);
								}
							}
							tileDrawers[i, j] = bufferQ;
						}

						// Old way:

				//		while ( tileQ.Count > 0 ) {
				//			if ( tileQ.Peek().Deleted() ) {
				//				tileQ.Dequeue();
							
				//			}
				//			else {
				//				break;
				//			}
				//		}

					}
				}
			}



			// delete all tile drawers:
			void clearAll()
			{
				for (int i = 0; i < gridSize; ++i ) {
					for (int j = 0; j < gridSize; ++j ) {
						Queue<TileDrawer> tileQ = tileDrawers[i, j];
						while ( tileQ != null && tileQ.Count != 0 ) {
							tileQ.Dequeue();
						}
					}
				}
			}



			public int GetScore()
			{
				return currentScore;
			}



			public int GetSize()
			{
				return gridSize;
			}



			// print indices of all empty cells in the grid:
			void printEmptyCells()
			{
				Log.LogInfo("=============");
				foreach (var cell in grid.GetEmptyCells()) {
					Log.LogInfo("Empty at [" + cell.First.ToString() + ", " + cell.Second.ToString() + "]");
				}
				Log.LogInfo("=============");
			}



			// convert grid indices into pixel coordinates:
			IntPair pixelCoord( int i, int j )
			{
				return new IntPair( (int)x + tilePadding + tileSize / 2 + ( tileSize + tilePadding) * j,
									(int)y + tilePadding + tileSize / 2 + ( tileSize + tilePadding) * i); 
			}



			// print all tile drawer states for debug:
			void printTileDrawers()
			{
				for (int i = 0; i < gridSize; ++i) {
					for (int j = 0; j < gridSize; ++j) {
						Queue<TileDrawer> tileQ = tileDrawers[i, j];
						if (tileQ.Count != 0) {
							Log.LogInfo( "[" + i.ToString() + ", " + j.ToString() + "]: length = " +
								tileQ.Count.ToString() + ":");
						
							foreach( var td in tileQ ) {
								TileDrawer tdFirst = tileQ.Peek();
								Log.LogInfo( td.GetString() + " "
									+ ( td.Deleted() ? "deleted" : "alive" )+ " First: " + tdFirst.GetString() + " "
									+ ( tdFirst.Deleted() ? "deleted" : "alive" ) );
							}

						}
					}
				}

			}



			// check for zero spawn events:
			void checkZS(string text)
			{
				for(int i = 0; i < gridSize; ++i) {
					for (int j = 0; j < gridSize; ++j) {
						Queue<TileDrawer> tileQ = tileDrawers[i, j];
						foreach(var td in tileQ) {
							if( td.GetString() == "0" ) {
								Console.WriteLine("Zero spawn");
								Console.WriteLine(text);
							}
						}
					}
				}
			}

		}
}
