using System.Collections.Generic;

namespace Shard;

public class Animator {

    private int frameDelay;
    private int frameCounter;
    private List<string> spritePathList;
    private string currentSpritePath;

    public Animator() {
        spritePathList = new List<string>();
        frameDelay = 0;
        frameCounter = 0;
    }

    public Animator(int frameDelay) {
        spritePathList = new List<string>();
        this.frameDelay = frameDelay;
        frameCounter = 0;
    }

    public string CurrentSpritePath {
        get => currentSpritePath;
        set => currentSpritePath = value;
    }

    public List<string> SpritePathList {
        get => spritePathList;
        set => spritePathList = value;
    }

    public void addSpritePath(string spritePath) {
        if (!spritePathList.Contains(spritePath)) {
            spritePathList.Add(spritePath);
        }
    }

    //I don't think we'll need this
    public void removeSpritePath(string spritePath) {
        if (spritePathList.Contains(spritePath)) {
            spritePathList.Remove(spritePath);
        }
        else {
            Debug.Log("[LISA] Tried to remove spritePath " + spritePath + " but it doesn't exist.");
        }
    }

    public string nextSprite() {

        if (frameDelay != 0) { //If there is a frame delay
            if (frameCounter < frameDelay) { //New frame is not needed yet, return current frame
                frameCounter++;
                return currentSpritePath;
            }

            if (frameCounter == frameDelay) { //New frame will be needed, reset counter
                frameCounter = 0;
            }
        }
        
        //Get next frame
        int index = spritePathList.IndexOf(currentSpritePath);
        int nextIndex = (index + 1) % (spritePathList.Count); 
        // Debug.Log("[LISA] current:" + spritePathList[index]);
        // Debug.Log("[LISA] next:" + spritePathList[nextIndex]);
                
        currentSpritePath = spritePathList[nextIndex];
        return spritePathList[nextIndex];
    }

}