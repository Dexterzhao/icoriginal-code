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
    class modeselect : MenuScreen
    {
        void normalmode(object sender, EventArgs e)
        {            
            ScreenManager.AddScreen(new Gameplay(0));
            ScreenManager.RemoveScreen(this);
 
        }
        void backtomenu(object s,EventArgs h)
        {
            ScreenManager.RemoveScreen(this);

        }
        void randommode(object s, EventArgs g)
        {
            ScreenManager.AddScreen(new Gameplay(1));
            ScreenManager.RemoveScreen(this);
        }
        void starmode(object s, EventArgs j)
        {
            ScreenManager.AddScreen(new Gameplay(2));
            ScreenManager.RemoveScreen(this);
        }
        
        public modeselect()
            : base("Mode")
        {
            MenuEntry normal = new MenuEntry("Normal Mode");
            MenuEntry random = new MenuEntry("Random Mode");
            MenuEntry star = new MenuEntry("Star Mode");
            MenuEntry back = new MenuEntry("back");

            normal.Selected += normalmode;
            random.Selected += randommode;
            star.Selected += starmode;
            back.Selected += backtomenu;

            MenuEntries.Add(normal);
            MenuEntries.Add(random);
            MenuEntries.Add(star); 
            MenuEntries.Add(back);
                            
        }
      
    }
}
