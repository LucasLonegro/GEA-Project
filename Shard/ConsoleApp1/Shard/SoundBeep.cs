/*
*
*   A very simple implementation of a very simple sound system.
*   @author Michael Heron
*   @version 1.0
*   
*/

using SDL2;
using System;
using System.Threading;

namespace Shard
{
    public class SoundSDL : Sound
    {
        private uint musicDevice = 0;
        private bool isMusicPlaying = false;
        private bool isMusicMuted = false;
        private bool isMusicPaused = false;
        private Thread? musicThread = null;

        // Play sound effects
        public override void playSound(string file)
        {
            SDL.SDL_AudioSpec have, want;
            uint length, dev;
            IntPtr buffer;

            file = Bootstrap.getAssetManager().getAssetPath(file);

            if (SDL.SDL_LoadWAV(file, out have, out buffer, out length) == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load sound file: {file}");
                return;
            }

            dev = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref have, out want, 0);
            if (dev == 0)
            {
                Console.WriteLine($"Failed to open sound device: {SDL.SDL_GetError()}");
                SDL.SDL_FreeWAV(buffer);
                return;
            }

            SDL.SDL_QueueAudio(dev, buffer, length);
            SDL.SDL_PauseAudioDevice(dev, 0);
            SDL.SDL_FreeWAV(buffer);
        }

        // Play background music in a loop
        public override void playBackgroundMusic(string file)
        {
            if (isMusicPlaying)
            {
                return;
            }

            SDL.SDL_AudioSpec have, want;
            uint length;
            IntPtr buffer;

            file = Bootstrap.getAssetManager().getAssetPath(file);

            if (SDL.SDL_LoadWAV(file, out have, out buffer, out length) == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load background music: {file}");
                return;
            }

            musicDevice = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref have, out want, 0);
            if (musicDevice == 0)
            {
                Console.WriteLine($"Failed to open music device: {SDL.SDL_GetError()}");
                SDL.SDL_FreeWAV(buffer);
                return;
            }

            isMusicPlaying = true;
            musicThread = new Thread(() =>
            {
                while (isMusicPlaying)
                {
                    SDL.SDL_ClearQueuedAudio(musicDevice);
                    SDL.SDL_QueueAudio(musicDevice, buffer, length);
                    if (!isMusicMuted) SDL.SDL_PauseAudioDevice(musicDevice, 0);

                    double durationMs = (length / (double)(have.freq * have.channels * (SDL.SDL_AUDIO_BITSIZE(have.format) / 8))) * 1000;
                    Thread.Sleep((int)durationMs);
                }

                SDL.SDL_CloseAudioDevice(musicDevice);
                SDL.SDL_FreeWAV(buffer);
            });

            musicThread.IsBackground = true;
            musicThread.Start();
        }
        
        public override void stopBackgroundMusic()//TODO: Not ready yet
        {
            if (!isMusicPlaying)
            {
                Console.WriteLine("No background music is playing.");
                return;
            }

            isMusicPlaying = false;
            musicThread?.Join();
            SDL.SDL_ClearQueuedAudio(musicDevice);
            SDL.SDL_CloseAudioDevice(musicDevice);
            musicDevice = 0;
            isMusicMuted = false;

            Console.WriteLine("Background music stopped.");
        }
        
        // Pause the background music
        public override void pauseBackgroundMusic()
        {
            if (!isMusicPlaying)
            {
                Console.WriteLine("Music is not playing.");
                return;
            }

            if (isMusicPaused)
            {
                Console.WriteLine("Music is already paused.");
                return;
            }

            SDL.SDL_PauseAudioDevice(musicDevice, 1);
            isMusicPaused = true;
        }

        // Unpause the background music
        public override void resumeBackgroundMusic()
        {
            if (!isMusicPlaying)
            {
                Console.WriteLine("No music to resume.");
                return;
            }

            if (!isMusicPaused)
            {
                Console.WriteLine("Music is already playing.");
                return;
            }

            SDL.SDL_PauseAudioDevice(musicDevice, 0);
            isMusicPaused = false;
        }
        
        // Mute the background music
        public override void muteBackgroundMusic()
        {
            if (isMusicPlaying && !isMusicMuted)
            {
                SDL.SDL_PauseAudioDevice(musicDevice, 1);
                isMusicMuted = true;
                Console.WriteLine("Mute");
            }
            else
            {
                Console.WriteLine("Background music is not playing or already muted.");
            }
        }

        // Unmute the background music
        public override void unmuteBackgroundMusic()
        {
            if (isMusicPlaying && isMusicMuted)
            {
                SDL.SDL_PauseAudioDevice(musicDevice, 0);
                isMusicMuted = false;
                Console.WriteLine("Unmute");
            }
            else
            {
                Console.WriteLine("Background music is not playing or already unmute.");
            }
        }
        
        //TODO: public override void setBackgroundVolume()
        //TODO: public override void setSoundVolume()
        //TODO: public override int getBackgroundVolume()
        //TODO: public override int getSoundVolume()
    }
}
