using GameTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using SDL2;

namespace Shard {
    class GameTest : Game , InputListener {
        GameObject background;
        List<GameObject> asteroids;

        public override void update() {
            Bootstrap.getDisplay()
                .showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10,
                    12, 255, 255, 255);
        }

        public override int getTargetFrameRate() {
            return 100;
        }

        public void createShips() {
            // 🎮 Player 1 Ship (Controlled as before)
            GameObject playerOne = new Spaceship(true);
            playerOne.Transform.X = 200;
            playerOne.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;
            playerOne.setAnimationEnabled();
            playerOne.Transform.addAnimation("go", [
                Bootstrap.getAssetManager().getAssetPath("spaceship.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship2.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship3.png")
            ], "KeyDown", (int) SDL.SDL_Scancode.SDL_SCANCODE_W);

            // 🎮 Player 2 Ship (Controlled with Arrow Keys)
            GameObject playerTwo = new Spaceship(false);
            playerTwo.Transform.X = Bootstrap.getDisplay().getWidth() - 200;;  // Different starting position
            playerTwo.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;

            GameObject asteroid;


//            asteroid.MyBody.Kinematic = true;
          
            background = new GameObject();
            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;
        }

        public override void initialize() {
            Bootstrap.getInput().addListener(this);
            createShips();

            
            Bootstrap.getSound().playBackgroundMusic("retroBackground.wav");

            asteroids = new List<GameObject>();
        }

        public void handleInput(InputEvent inp, string eventType) {
            if (eventType == "MouseDown") {
                Console.WriteLine("Pressing button " + inp.Button);
                
                if (inp.Button == 1) {
                    Asteroid asteroid = new Asteroid();
                    asteroid.Transform.X = inp.X;
                    asteroid.Transform.Y = inp.Y;
                    asteroids.Add(asteroid);
                }

                if (inp.Button == 3) {
                    foreach (GameObject ast in asteroids) {
                        ast.ToBeDestroyed = true;
                    }

                    asteroids.Clear();
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