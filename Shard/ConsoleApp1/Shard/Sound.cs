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
        abstract public void playSound(string file);
        abstract public void playBackgroundMusic(string file);
        abstract public void stopBackgroundMusic();
        abstract public void pauseBackgroundMusic();
        abstract public void resumeBackgroundMusic();
        abstract public void muteBackgroundMusic();
        abstract public void unmuteBackgroundMusic();
        
        //TODO: abstract public void setBackgroundVolume()
        //TODO: abstract public void setSoundVolume()
        //TODO: abstract public int getBackgroundVolume()
        //TODO: abstract public int getSoundVolume()
    }
}
