﻿using System;
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

namespace Overpopulated
{
    public class Overpopulated : Game
    {
        /// <summary>
        /// Overpopulated constructor
        /// </summary>
        public Overpopulated()
            : base()
        {
            //	root directory for standard x64 C# application
            Parameters.ContentDirectory = @"..\..\..\Content";

            //	enable object tracking :
            Parameters.TrackObjects = true;

            //	enable developer console :
            Parameters.Developer = false;

            //	enable debug graphics device in Debug :
#if DEBUG
				Parameters.UseDebugDevice	=	true;
#endif

            //	add services :
            AddService(new SpriteBatch(this), false, false, 0, 0);
            AddService(new DebugStrings(this), true, true, 9999, 9999);
            AddService(new DebugRender(this), true, true, 9998, 9998);
			AddService(new UserInterface(this, "stencil"), true, true, 2, 2);
			AddService(new GridDrawer(this),  true, true, 1, 1);

            //	add here additional services :

            //	load configuration for each service :
            LoadConfiguration();

            //	make configuration saved on exit :
            Exiting += FusionGame_Exiting;
        }


		SpriteFont uiFont;


        /// <summary>
        /// Add services :
        /// </summary>
        protected override void Initialize()
        {
            //	initialize services :
            base.Initialize();
			uiFont = Content.Load<SpriteFont>("stencil");


			var Grid = GetService<GridDrawer>();

			var UI = GetService<UserInterface>();
			UI.RootFrame = new Frame(this, 0, 0, 800, 600, "", Color.Zero)
			{
				Border = 1,
                BorderColor = Color.White
			};


			Grid.InitUI(UI.RootFrame);

            //	add keyboard handler :
            InputDevice.KeyDown += InputDevice_KeyDown;

        }



        /// <summary>
        /// Handle keys for each demo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputDevice_KeyDown(object sender, Fusion.Input.InputDevice.KeyEventArgs e)
        {
            if (e.Key == Keys.F1)
            {
                ShowEditor();
            }

            if (e.Key == Keys.F5)
            {
                BuildContent();
                Content.Reload<Texture2D>();
            }

            if (e.Key == Keys.F7)
            {
                BuildContent();
                Content.ReloadDescriptors();
            }

            if (e.Key == Keys.F12)
            {
                GraphicsDevice.Screenshot();
            }

            if (e.Key == Keys.Escape)
            {
                Exit();
            }
        }



        /// <summary>
        /// Save configuration on exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FusionGame_Exiting(object sender, EventArgs e)
        {
            SaveConfiguration();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            var ds = GetService<DebugStrings>();


			var Grid = GetService<GridDrawer>();

			
            ds.Add(Color.Orange, "FPS {0}", gameTime.Fps);
    //        ds.Add("F1   - show developer console");
    //        ds.Add("F5   - build content and reload textures");
    //        ds.Add("F12  - make screenshot");
            ds.Add("ESC  - exit");

            base.Update(gameTime);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="stereoEye"></param>
        protected override void Draw(GameTime gameTime, StereoEye stereoEye)
        {
            base.Draw(gameTime, stereoEye);
        }
    }
}
