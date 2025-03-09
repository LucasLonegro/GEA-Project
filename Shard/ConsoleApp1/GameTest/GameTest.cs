using GameTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SDL2;
using Steamworks;

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
        
        private static void writeSteamAppIdTxtFile() {
            string strCWDPath = Directory.GetCurrentDirectory();
            string strSteamAppIdPath = Path.Combine(strCWDPath, "steam_appid.txt");

            // If the steam_appid.txt file already exists, then there's nothing to do.
            if (File.Exists(strSteamAppIdPath)) {
                return;
            }

            Debug.Log("[Steamworks.NET] 'steam_appid.txt' is not present in the project root. Writing...");

            try {
                StreamWriter appIdFile = File.CreateText(strSteamAppIdPath);
                appIdFile.Write("480");
                appIdFile.Close();

                Debug.Log("[Steamworks.NET] Successfully copied 'steam_appid.txt' into the project root.");
            }
            catch (System.Exception e) {
                Console.WriteLine("[Steamworks.NET] Could not copy 'steam_appid.txt' into the project root. Please place 'steam_appid.txt' into the project root manually.");
                Console.WriteLine(e);
            }
        }

        public void createShips() {
            // 🎮 Player 1 Ship (Controlled as before)
            GameObject playerOne = new Spaceship(true);
            playerOne.Transform.X = 200;
            playerOne.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;

            // 🎮 Player 2 Ship (Controlled with Arrow Keys)
            GameObject playerTwo = new Spaceship(false);
            playerTwo.Transform.X = Bootstrap.getDisplay().getWidth() - 200;;  // Different starting position
            playerTwo.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;

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
            
            Console.WriteLine("Steam is running " + SteamAPI.IsSteamRunning());
            writeSteamAppIdTxtFile();
            try
            {
                if (SteamAPI.Init())
                {
                    Console.WriteLine("SteamworksHelper init success");
                }
                else
                {
                    Console.WriteLine("SteamworksHelper init failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e);
            }
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