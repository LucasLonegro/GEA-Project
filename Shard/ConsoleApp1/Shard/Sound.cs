/*
*
*   This class intentionally left blank.  
*   @author Michael Heron
*   @version 1.0
*   
*/

namespace Shard
{
    abstract public class Sound
    {
        abstract public void playBackgroundMusic(string file);
        abstract public void playSound(string file);
        abstract public void stopBackgroundMusic();
        abstract public void pauseBackgroundMusic();
        abstract public void resumeBackgroundMusic();
        abstract public void muteBackgroundMusic();
        abstract public void unmuteBackgroundMusic();
        abstract public void setBackgroundVolume(float volume);
        abstract public int getBackgroundVolume();
        abstract public void setSoundVolume(float volume);
        abstract public int getSoundVolume();
    }
}
