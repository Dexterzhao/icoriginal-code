using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Freeze;
using System.Threading;
using Microsoft.Xna.Framework;
namespace Freeze
{
    class Loadscreen:GameScreen
    {
        private Thread backgroundThread;
         public void Loadingscreen()
         {
             TransitionOnTime = TimeSpan.FromSeconds(0.0);
             TransitionOffTime = TimeSpan.FromSeconds(0.0);
         }
        void BackgroundLoadContent()
        {
            //插入需要载入的数据图片
            ScreenManager.Game.Content.Load<object>("background");
            ScreenManager.Game.Content.Load<object>("title");
            ScreenManager.Game.Content.Load<object>("menufont");
            
        }
    public override void LoadContent()
        {
        if (backgroundThread == null)
            {
            backgroundThread = new Thread(BackgroundLoadContent);
            backgroundThread.Start();
            }
            base.LoadContent();
        }
    public override void Update(GameTime gametime,bool otherScreenHasFocus, bool coveredbyotherscreen)
        {
            if (backgroundThread!= null && backgroundThread.Join(10))
                {
                    backgroundThread = null;
                    this.ExitScreen();
                    ScreenManager.AddScreen(new MainMenuScreen());
                    ScreenManager.Game.ResetElapsedTime();
                }
            base.Update(gametime,otherScreenHasFocus, coveredbyotherscreen);
        }
    }
}
