using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Freeze;
using Microsoft.Devices.Sensors;
namespace Freeze
{
    /// <summary>
    /// 这是游戏的主类型
    /// </summary>
    public class Freeze : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenmanager;

        public Freeze()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;

            // Windows Phone 的默认帧速率为 30 fps。
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            screenmanager = new ScreenManager(this);
            Components.Add(screenmanager);

            screenmanager.AddScreen(new BackgroundScreen());
            screenmanager.AddScreen(new Loadscreen());

        }
        
    }
       
}
