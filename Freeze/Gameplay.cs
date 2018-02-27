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
using Microsoft.Devices;

namespace Freeze
{
    class Gameplay : GameScreen
    {
        int modeNumber = 0;
        GraphicsDeviceManager graphics;


        Texture2D playerObject;
        Vector2 playerPosition;
        Vector2 playerSize;
        Vector2 playerSpeed = new Vector2(0f, 0f);
        Vector2 playerAcc = new Vector2(5.0f, 3.0f);
        Vector2 midPoint;
        Vector2 scorePoint, timePoint;
        int score;
        float scoreZoneRadius;
        const float scoreZoneRadiusIndexByFps = 0.9996f;
        const float maxAccIndexByFps = 1.0008f;
        const float scoreIndexByFps = 1.0005f;
        const float heightIndexByFps = 1.001f;
        float midPointHeight;
        float scoreIndex;
        float maxAcceleration;
        float gravityAcceleration;
        SpriteFont scoreFont;
        int maxTime = 120;  // 游戏时间120s
        float X, Y, Z;
        Accelerometer accEmulator;

        SoundEffect soundEffect;
        SoundEffect starEffect;
        Texture2D starObject;
        Vector2 starPosition;
        Vector2 starSize;
        bool starCaught;

        public Gameplay(int mode)
        {
            modeNumber = mode;
            

        }

        public override void Initialize()
        {
            // TODO: 在此处添加初始化逻辑

            // 初始化重力感应加速器
            accEmulator = new Accelerometer();
            accEmulator.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(AccelerometerReadingChanged);
            accEmulator.Start();

            // 暂定必须横屏
           //  SupportedOrientations = DisplayOrientation.LandscapeLeft;
            base.Initialize();
        }
        
        public override void LoadContent()
        {

            playerObject =ScreenManager.Game.Content.Load<Texture2D>("Game");

            soundEffect = ScreenManager.Game.Content.Load<SoundEffect>("Windows Ding");

            scoreFont = ScreenManager.Game.Content.Load<SpriteFont>("ScoreFont");

            playerSize.X = playerObject.Bounds.Width;
            playerSize.Y = playerObject.Bounds.Height;

            playerPosition = playerSize / 2;

            midPoint.X = ScreenManager.Game.GraphicsDevice.Viewport.Width / 2;
            midPoint.Y = ScreenManager.Game.GraphicsDevice.Viewport.Height / 2;

            scorePoint.X = 0;
            scorePoint.Y = 0;

            timePoint.X = 660;
            timePoint.Y = 0;

            scoreZoneRadius = midPoint.X;

            maxAcceleration = 10.0f;

            if (modeNumber == 2)
            {
                starObject =ScreenManager.Game.Content.Load<Texture2D>("StarObject");

                starEffect =ScreenManager.Game.Content.Load<SoundEffect>("Warning");

                starCaught = false;

                starSize.X = starObject.Bounds.Width;
                starSize.Y = starObject.Bounds.Height;

                scoreZoneRadius = Vector2.Distance(scorePoint, midPoint);

                Random randomSeed = new Random();
                starPosition.X = randomSeed.Next(Convert.ToInt32(starSize.X), ScreenManager.GraphicsDevice.Viewport.Width);
                starPosition.Y = randomSeed.Next(Convert.ToInt32(starSize.Y), ScreenManager.GraphicsDevice.Viewport.Height);
            }

            else
            {
                scoreZoneRadius = midPoint.Y;
            }

            scoreIndex = 1;

            midPointHeight = 1 / 500;
        }
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // 允许游戏退出
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
               // foreach (GameScreen screen in ScreenManager.GetScreens())
                ScreenManager.Game.ResetElapsedTime();
                    ScreenManager.RemoveScreen(this);
               // ScreenManager.AddScreen(new BackgroundScreen());
               // ScreenManager.AddScreen(new MainMenuScreen());
            }


            if (gameTime.TotalGameTime.TotalSeconds >= maxTime)
            {
                foreach (GameScreen screen in ScreenManager.GetScreens()) screen.ExitScreen();
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }

            // 半径缩小（追星模式不需要）
            if (modeNumber != 2)
            {
                scoreZoneRadius *= scoreZoneRadiusIndexByFps;
            }

            // 速度变化（函数需要重写）
            playerAcc.X = -Y * 20;
            playerAcc.Y = -X * 20;
            gravityAcceleration = Vector2.Distance(Vector2.Zero, playerAcc);

            if (gravityAcceleration > maxAcceleration)
            {
                playerAcc.X *= maxAcceleration / gravityAcceleration;
                playerAcc.Y *= maxAcceleration / gravityAcceleration;
            }

            playerSpeed += playerAcc;
            playerSpeed.X += (playerPosition.X > midPoint.X ? (ScreenManager.Game.GraphicsDevice.Viewport.Width - playerPosition.X) : -playerPosition.X) * midPointHeight;
            playerSpeed.Y += (playerPosition.Y > midPoint.Y ? (ScreenManager.Game.GraphicsDevice.Viewport.Height - playerPosition.Y) : -playerPosition.Y) * midPointHeight;

            // 高度变化
            midPointHeight *= heightIndexByFps;

            // 最大加速度变化
            maxAcceleration *= maxAccIndexByFps;

            UpdateSprite(gameTime, ref playerPosition, ref playerSpeed);

            // 碰撞检测
            if (modeNumber == 2)
            {
                CheckForCollision(ref playerPosition, ref playerSize, ref starPosition, ref starSize);
            }

            // 得分
            if (modeNumber == 0)
            {
                double tempScore = scoreZoneRadius - Vector2.Distance(playerPosition, midPoint);
                score += Convert.ToInt32(tempScore > 0 ? tempScore * scoreIndex : 0);
            }

            else if (modeNumber == 2 && starCaught)
            {
                double tempScore = scoreZoneRadius - Vector2.Distance(playerPosition, midPoint);
                score += Convert.ToInt32(tempScore > 0 ? tempScore * scoreIndex : 0) * 10;
            }

            // 得分倍数变化
            scoreIndex *= scoreIndexByFps;

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen );
        }
        public override void Draw(GameTime gameTime)
        {
           ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: 在此处添加绘图代码

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(playerObject, midPoint - playerSize / 2, Color.White);
            ScreenManager.SpriteBatch.End();

            // 还是让赵神找个好看一点的粗线圈圆，不停地改变尺寸吧，画圆太麻烦了
            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //spriteBatch.Draw(playerObject, midPoint, Color.White);
            //spriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(playerObject, playerPosition - playerSize / 2, Color.Blue);
            ScreenManager.SpriteBatch.End();

            if (modeNumber == 2)
            {
                if (starCaught)
                {
                    starCaught = false;

                    Random randomSeed = new Random();
                    starPosition.X = randomSeed.Next(Convert.ToInt32(starSize.X), ScreenManager.GraphicsDevice.Viewport.Width);
                    starPosition.Y = randomSeed.Next(Convert.ToInt32(starSize.Y), ScreenManager.GraphicsDevice.Viewport.Height);
                }

                ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                ScreenManager.SpriteBatch.Draw(starObject, starPosition - starSize / 2, Color.Gold);
                ScreenManager.SpriteBatch.End();
            }

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(scoreFont, "Score : " + score.ToString(), scorePoint, Color.White);
            ScreenManager.SpriteBatch.DrawString(scoreFont, "Time : " + gameTime.TotalGameTime.Minutes.ToString() + ":" + gameTime.TotalGameTime.Seconds.ToString(), timePoint, Color.White);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
        float SpeedChange(int time, float nowspeed, float input)
        {
            float Acceleration = 0;
            if (input * nowspeed > 0)
                Acceleration = nowspeed * input;

            if (input * nowspeed < 0)
                Acceleration =- Math.Abs( nowspeed) / (1-Math.Abs( input));
            
            if (nowspeed == 0)
                Acceleration = input * 4;
            

            return Acceleration;

        }
        void UpdateSprite(GameTime gameTime, ref Vector2 spritePosition, ref Vector2 spriteSpeed)
        {
            // Move the sprite by speed, scaled by elapsed time.
            spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int MaxX = ScreenManager.GraphicsDevice.Viewport.Width;
            int MinX = 0;
            int MaxY = ScreenManager.GraphicsDevice.Viewport.Height;
            int MinY = 0;

            // Check for bounce.
            if (spritePosition.X > MaxX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MaxX;

                soundEffect.Play();
               // VibrateController.Default.Start(TimeSpan.FromSeconds(1));
            }

            else if (spritePosition.X < MinX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = MinX;

                soundEffect.Play();
                //VibrateController.Default.Start(TimeSpan.FromSeconds(1));
            }

            if (spritePosition.Y > MaxY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MaxY;

                soundEffect.Play();
              //  VibrateController.Default.Start(TimeSpan.FromSeconds(1));
            }

            else if (spritePosition.Y < MinY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = MinY;

                soundEffect.Play();
                //VibrateController.Default.Start(TimeSpan.FromSeconds(1));
            }

        }
        public override void UnloadContent()
        {
            // TODO: 在此处取消加载任何非 ContentManager 内容
            accEmulator.Stop();
        }

        void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            //触发UI更新  
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => NewReading(e));
        }
        // 重力感应数据
        void NewReading(AccelerometerReadingEventArgs e)
        {
            X = (float)e.X;
            Y = (float)e.Y;
            Z = (float)e.Z;
        }
        void CheckForCollision(ref Vector2 sprite1Position, ref Vector2 sprite1Size, ref Vector2 sprite2Position, ref Vector2 sprite2Size)
        {
            BoundingBox bb1 = new BoundingBox(new Vector3(sprite1Position.X - (sprite1Size.X / 2), sprite1Position.Y - (sprite1Size.Y / 2), 0), new Vector3(sprite1Position.X + (sprite1Size.X / 2), sprite1Position.Y + (sprite1Size.Y / 2), 0));

            BoundingBox bb2 = new BoundingBox(new Vector3(sprite2Position.X - (sprite2Size.X / 2), sprite2Position.Y - (sprite2Size.Y / 2), 0), new Vector3(sprite2Position.X + (sprite2Size.X / 2), sprite2Position.Y + (sprite2Size.Y / 2), 0));

            if (bb1.Intersects(bb2))
            {
                starEffect.Play();

                starCaught = true;
            }

        }
        
    }

}
