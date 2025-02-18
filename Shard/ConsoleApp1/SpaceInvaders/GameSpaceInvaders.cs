using SpaceInvaders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Shard
{
    class GameSpaceInvaders : Game, InputListener
    {
        private Invader[,] myInvaders;
        private int xdir;
        private int moveCounter, moveDir;
        private float animCounter;
        private float timeToSwap;
        private int columns, rows;
        private int invaderCount;
        private bool dead;
        private List<Invader> livingInvaders;
        private Random rand;
        private GameObject ship;
        private SoundSDL soundSystem = new SoundSDL();
        private SoundSDL backGroundSound = new SoundSDL();
        
        public int Xdir { get => xdir; set => xdir = value; }
        public bool Dead { get => dead; set => dead = value; }

        public override bool isRunning()
        {
            if (ship == null || ship.ToBeDestroyed == true)
            {
                return false;
            }

            return true;

        }
        public override void update()
        {
            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getFPS(), 10, 10, 12, 255, 255, 255);
            
            int ymod = 0;
            int deaths = 0;
            
            soundSystem.playBackgroundMusic("retroBackground.wav");
            if (isRunning() == false)
            {   
                soundSystem.pauseBackgroundMusic();
                soundSystem.playSound("gameOver.wav");
                Thread.Sleep(2000);
                Color col = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                Bootstrap.getDisplay().showText("GAME OVER!", 300, 300, 128, col);
                soundSystem.stopBackgroundMusic();
                return;
            }
            animCounter += (float)Bootstrap.getDeltaTime();

            //            Debug.Log("Move Counter is " + moveCounter + ", dir is " + moveDir);

            if (animCounter > timeToSwap)
            {
                animCounter -= timeToSwap;

                livingInvaders.Clear();
                if (moveCounter > 12 || moveCounter <= -2)
                {
                    ymod = 50;
                    Xdir *= -1;
                    moveDir *= -1;

                }

                moveCounter += moveDir;


                invaderCount = 0;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (myInvaders[i, j] == null || myInvaders[i, j].ToBeDestroyed == true)
                        {
                            deaths += 1;
                            continue;
                        }

                        // Speed them up as their numbers diminish.
                        timeToSwap = 2 - ((deaths / 3) * 0.1f);

                        myInvaders[i, j].changeSprite();

                        if (ymod != 0)
                        {
                            myInvaders[i, j].Transform.translate(0, ymod);
                        }
                        else
                        {
                            myInvaders[i, j].Transform.translate(xdir, 0);
                        }

                        livingInvaders.Add(myInvaders[i, j]);

                    }
                }

                Debug.Log("Living invaders" + livingInvaders.Count);

                // Pick a random invader to fire.
                livingInvaders[rand.Next(livingInvaders.Count)].fire();




            }

        }

        public void createObjects()
        {
            ship = new Spaceship();


            int ymod = 0;

            timeToSwap = 3;
            invaderCount = 0;
            livingInvaders = new List<Invader>();
            for (int j = 0; j < rows; j++)
            {
                for (int i = 0; i < columns; i++)
                {
                    Invader invader = new Invader();
                    invader.Transform.X = 100 + (i * 50);
                    invader.Transform.Y = 100 + (ymod * 50);

                    myInvaders[j, i] = invader;

                    livingInvaders.Add(myInvaders[j, i]);

                    invaderCount += 1;
                }

                ymod += 1;
            }

            moveDir = 1;
            Xdir = 35;

            // Finally, four bunkers

            for (int i = 0; i < 4; i++)
            {

                Bunker b = new Bunker();

                b.Transform.X = 200 + (i * 180);
                b.Transform.Y = 600;

                Debug.Log("Setting up bunker " + i + "at " + b.Transform.X + ", " + b.Transform.Y);

                b.setupBunker();

            }

            rand = new Random();

        }

        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            rows = 6;
            columns = 11;

            myInvaders = new Invader[rows, columns];
            createObjects();

            Debug.Log("Bing!");


        }

        public void handleInput(InputEvent inp, string eventType)
        {


        }
    }
}
