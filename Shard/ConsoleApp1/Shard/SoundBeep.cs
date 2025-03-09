using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{
    public class SoundSDL : Sound
    {
        private float backgroundVolume = 1.0f;
        private float soundVolume = 1.0f;
        
        private IntPtr backgroundMusic; // Store music
        private Dictionary<string, IntPtr> soundEffects; // Store sound effects

        public SoundSDL()
        {
            if (SDL_mixer.Mix_OpenAudio(44100, SDL.AUDIO_S16SYS, 2, 2048) < 0)
            {
                Console.WriteLine($"SDL_Mixer Initialization Error: {SDL.SDL_GetError()}");
            }

            soundEffects = new Dictionary<string, IntPtr>();
        }

        // Load and play background music
        public override void playBackgroundMusic(string file)
        {
            file = Bootstrap.getAssetManager().getAssetPath(file);

            if (backgroundMusic != IntPtr.Zero)
            {
                SDL_mixer.Mix_HaltMusic();
                SDL_mixer.Mix_FreeMusic(backgroundMusic);
            }

            backgroundMusic = SDL_mixer.Mix_LoadMUS(file);
            if (backgroundMusic == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load background music: {file}");
                return;
            }

            SDL_mixer.Mix_PlayMusic(backgroundMusic, -1); // Play in loop (-1 means infinite)
            SDL_mixer.Mix_VolumeMusic((int)(SDL_mixer.MIX_MAX_VOLUME * backgroundVolume));

        }

        // Play a sound effect
        public override void playSound(string file)
        {
            file = Bootstrap.getAssetManager().getAssetPath(file);

            if (!soundEffects.ContainsKey(file))
            {
                IntPtr sound = SDL_mixer.Mix_LoadWAV(file);
                if (sound == IntPtr.Zero)
                {
                    Console.WriteLine($"Failed to load sound: {file}");
                    return;
                }
                soundEffects[file] = sound;
            }

            SDL_mixer.Mix_PlayChannel(-1, soundEffects[file], 0); // -1 = Play on any free channel
            SDL_mixer.Mix_Volume(-1, (int)(SDL_mixer.MIX_MAX_VOLUME * soundVolume));
        }
        
        // Stop background music
        public override void stopBackgroundMusic()
        {
            SDL_mixer.Mix_HaltMusic();
        }
        
        // Pause background music
        public override void pauseBackgroundMusic()
        {
            if (SDL_mixer.Mix_PlayingMusic() == 1)
            {
                SDL_mixer.Mix_PauseMusic();
                Console.WriteLine("Background music paused.");
            }
        }
 
        // Resume background music
        public override void resumeBackgroundMusic()
        {
            if (SDL_mixer.Mix_PausedMusic() == 1)
            {
                SDL_mixer.Mix_ResumeMusic();
                Console.WriteLine("Background music resumed.");
            }
        }

        // Mute background music
        public override void muteBackgroundMusic()
        {
            SDL_mixer.Mix_VolumeMusic(0);
        }

        // Unmute background music
        public override void unmuteBackgroundMusic()
        {
            SDL_mixer.Mix_VolumeMusic((int)(SDL_mixer.MIX_MAX_VOLUME * backgroundVolume));
        }

        // Set background music volume (real-time)
        public override void setBackgroundVolume(float volume)
        {
            backgroundVolume = Math.Clamp(volume, 0.0f, 1.0f);
            SDL_mixer.Mix_VolumeMusic((int)(SDL_mixer.MIX_MAX_VOLUME * backgroundVolume));

            Console.WriteLine($"Background music volume set to {backgroundVolume * 100}%.");
        }
        
        // Get current background music volume
        public override int getBackgroundVolume()
        {
            return (int)((float)SDL_mixer.Mix_VolumeMusic(-1) / SDL_mixer.MIX_MAX_VOLUME * 100);
        }

        // Set sound effect volume
        public override void setSoundVolume(float volume)
        {
            soundVolume = Math.Clamp(volume, 0.0f, 1.0f);
            SDL_mixer.Mix_Volume(-1, (int)(SDL_mixer.MIX_MAX_VOLUME * soundVolume));

            Console.WriteLine($"Sound effects volume set to {soundVolume * 100}%.");
        }
        
        // Get current sound volume
        public override int getSoundVolume()
        {
            return (int)((float)SDL_mixer.Mix_Volume(-1, -1) / SDL_mixer.MIX_MAX_VOLUME * 100);
        }

    }
}
