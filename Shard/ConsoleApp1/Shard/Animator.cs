using System.Collections.Generic;

namespace Shard;

public class Animator {

    private List<string> spritePathList;
    private string currentSpritePath;
    private bool enabled;
    private int frameDelay;
    private int ctr;

    public Animator(List<string> spritePathList, int frameDelay) {
        this.spritePathList = spritePathList;
        currentSpritePath = spritePathList[0];
        this.frameDelay = frameDelay;
    }

    public bool Enabled {
        get => enabled;
        set => enabled = value;
    }

    public int FrameDelay {
        get => frameDelay;
        set => frameDelay = value;
    }

    public string CurrentSpritePath {
        get => currentSpritePath;
        set => currentSpritePath = value;
    }

    public List<string> SpritePathList {
        get => spritePathList;
        set => spritePathList = value;
    }

    //I don't think we'll need this
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
            Debug.Log("Tried to remove spritePath " + spritePath + " but it doesn't exist.");
        }
    }

    public string nextSprite() {
        
        int index = spritePathList.IndexOf(currentSpritePath);
        int nextIndex = (index + 1) % (spritePathList.Count);
        string nextSprite = spritePathList[nextIndex];

        if (ctr >= frameDelay) {
            ctr = 0;
            currentSpritePath = nextSprite;
            return nextSprite;
        }
        
        ctr++;
        return currentSpritePath;
        
    }

}