using GameTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using SDL2;

namespace Shard {
    class GameTest : Game, InputListener {
        GameObject background;
        List<GameObject> asteroids;
        private SoundSDL soundSystem = new SoundSDL();


        public override void update() {
            Bootstrap.getDisplay()
                .showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10,
                    12, 255, 255, 255);
        }

        public override int getTargetFrameRate() {
            return 100;
        }

        public void createShip() {
            GameObject ship = new Spaceship();
            Random rand = new Random();
            int offsetx = 0, offsety = 0;

            GameObject asteroid;


//            asteroid.MyBody.Kinematic = true;


            background = new GameObject();
            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;
        }

        public override void initialize() {
            Bootstrap.getInput().addListener(this);
            createShip();

            soundSystem.playBackgroundMusic("retroBackground.wav");

            asteroids = new List<GameObject>();

            soundSystem.playBackgroundMusic("retroBackground.wav");
        }

        public void handleInput(InputEvent inp, string eventType) {
            if (eventType == "MouseDown") {
                Console.WriteLine("Pressing button " + inp.Button);
            }

            if (eventType == "MouseDown" && inp.Button == 1) {
                Asteroid asteroid = new Asteroid();
                asteroid.Transform.X = inp.X;
                asteroid.Transform.Y = inp.Y;
                asteroids.Add(asteroid);
            }

            if (eventType == "MouseDown" && inp.Button == 3) {
                foreach (GameObject ast in asteroids) {
                    ast.ToBeDestroyed = true;
                }

                asteroids.Clear();
            }
            
            if (eventType == "KeyUp") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_1) {
                    Bootstrap.getDisplay().scaleDown();
                }
            }
            
            if (eventType == "KeyUp") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_2) {
                    Bootstrap.getDisplay().scaleUp();
                }
            }
        }
    }
}