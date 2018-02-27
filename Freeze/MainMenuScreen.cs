using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Freeze;
using Microsoft.Devices.Sensors;
namespace Freeze
{
    class MainMenuScreen:MenuScreen
    {
        void startgamemenuentrySelected(object sender, EventArgs e)
        {            
            ScreenManager.AddScreen(new modeselect());
            
        }
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }
        void scoreSelected(object s, EventArgs g)
        {
           // ScreenManager.AddScreen(new HighScore());
        }
        
        public MainMenuScreen()
            : base("Main")
        {
            MenuEntry startgamemenuentry = new MenuEntry("Start Game");
            MenuEntry exitmenuentry = new MenuEntry("quit");
            MenuEntry score = new MenuEntry("score");

            score.Selected += scoreSelected;
            startgamemenuentry.Selected += startgamemenuentrySelected;
            exitmenuentry.Selected += OnCancel;

            MenuEntries.Add(startgamemenuentry);
            MenuEntries.Add(score);
            MenuEntries.Add(exitmenuentry); 
                            
        }
      
    }
}
