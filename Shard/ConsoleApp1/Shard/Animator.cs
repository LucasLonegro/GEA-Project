using System.Collections.Generic;

namespace Shard;

public class Animator {

    private List<string> spritePathList;
    private string currentSpritePath;

    public Animator() {
        spritePathList = new List<string>();
    }

    public string CurrentSpritePath {
        get => currentSpritePath;
        set => currentSpritePath = value;
    }

    public List<string> SpritePathList {
        get => spritePathList;
        set => spritePathList = value;
        //No setter bc we only want 1 list during the duration of the run!
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
        int index = spritePathList.IndexOf(currentSpritePath);
        int nextIndex = (index + 1) % (spritePathList.Count); 
        // Debug.Log("[LISA] current:" + spritePathList[index]);
        // Debug.Log("[LISA] next:" + spritePathList[nextIndex]);
                
        currentSpritePath = spritePathList[nextIndex];
        return spritePathList[nextIndex];
    }

}