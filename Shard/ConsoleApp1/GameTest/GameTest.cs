using GameTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using SDL2;

namespace Shard {
    class GameTest : Game , InputListener {
        GameObject background;
        List<GameObject> balls;
        private Ball theBall;
        private Goalpost goal1;
        private Goalpost goal2;

        public override void update() {
            Bootstrap.getDisplay()
                .showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 
                    10, 10, 30, 255, 255, 255);
            Bootstrap.getDisplay().showText(
                goal1.GoalsScored + " / " + goal2.GoalsScored, 
                1200, 10, 30, 255, 255, 255);
        }

        public override int getTargetFrameRate() {
            return 100;
        }

        public void createShips() {
            
            // 🎮 Player 1 Ship (Controlled as before)
            GameObject playerOne = new Spaceship(true);
            playerOne.Transform.X = 200;
            playerOne.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;
            playerOne.Transform.SpritePath =
                Bootstrap.getAssetManager().getAssetPath("spaceshipA.png");

            // 🎮 Player 2 Ship (Controlled with Arrow Keys)
            GameObject playerTwo = new Spaceship(false);
            playerTwo.Transform.X = Bootstrap.getDisplay().getWidth() - 200;  // Different starting position
            playerTwo.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;
            playerTwo.Transform.SpritePath =
                Bootstrap.getAssetManager().getAssetPath("spaceshipB.png");
            
            //Adding the animations
            Animator goAnimation = new Animator([
                Bootstrap.getAssetManager().getAssetPath("spaceship.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship2.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship3.png")
            ], "KeyDown", (int)SDL.SDL_Scancode.SDL_SCANCODE_W, 30);
            Animator goAnimation2 = new Animator([
                Bootstrap.getAssetManager().getAssetPath("spaceshipA.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceshipB.png"),
            ], "KeyDown", (int)SDL.SDL_Scancode.SDL_SCANCODE_UP, 30);
            
            playerOne.addAnimation("go", goAnimation);
            playerTwo.addAnimation("go", goAnimation2);
            
            // background = new GameObject();
            // background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            // background.Transform.X = 0;
            // background.Transform.Y = 0;
        }

        public void createGoalposts()
        {
            goal1 = new Goalpost();
            goal1.Transform.X = 50.0f;
            goal1.Transform.Y = 300.0f;
            goal1.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("LeftGoalpost.png");
            
            goal2 = new Goalpost();
            goal2.Transform.X = 1150.0f;
            goal2.Transform.Y = 300.0f;
            goal2.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("RightGoalpost.png");
        }

        public override void initialize() {
            Bootstrap.getInput().addListener(this);
            createShips();
            createGoalposts();
            
            theBall = new Ball();
            theBall.Transform.X = Bootstrap.getDisplay().getWidth() / 2;
            theBall.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;
            balls = new List<GameObject>();
            balls.Add(theBall);
            
            Bootstrap.getSound().playBackgroundMusic("retroBackground.wav");
        }
        

        public void handleInput(InputEvent inp, string eventType) {
            if (eventType == "MouseDown") {
                Console.WriteLine("Pressing button " + inp.Button);
                
                if (inp.Button == 1) {
                    Ball ball = new Ball();
                    ball.Transform.X = inp.X;
                    ball.Transform.Y = inp.Y;
                    balls.Add(ball);
                }

                if (inp.Button == 3) {
                    foreach (GameObject ast in balls) {
                        ast.ToBeDestroyed = true;
                    }

                    balls.Clear();
                }
            }

            
            //SCALING THE WINDOW
            if (eventType == "KeyUp") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_1) {
                    Bootstrap.getDisplay().scaleWindow(Scaling.Down);
                }
            }
            
            if (eventType == "KeyUp") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_2) {
                    Bootstrap.getDisplay().scaleWindow(Scaling.Up);
                }
            }
        }
    }
}