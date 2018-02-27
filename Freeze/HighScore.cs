using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Devices;

namespace Freeze
{
    class HighScore : GameScreen
    {
        SpriteFont font;

        public override void LoadContent()
        {
            font = ScreenManager.Game.Content.Load<SpriteFont>("ScoreFont");
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        public override void Draw(GameTime gametime)
        {
            Vector2 writeposition = new Vector2(400, 290);
            

            ScreenManager.SpriteBatch.Begin();

            for (int i = 0; i <= 4; i++)
            {
                ScreenManager.SpriteBatch.DrawString(font,"Rank"+ i.ToString()+ ":300"   , writeposition, Color.White);
                writeposition.Y -= 90;
            }

            ScreenManager.SpriteBatch.End();

            this.Draw(gametime);
        }
    }
}
