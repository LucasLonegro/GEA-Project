using System.Collections.Generic;

namespace Shard;

public class Animator : InputListener {
    private List<string> spritePathList;
    private string currentSpritePath;
    private bool enabled;
    private int frameDelay;
    private int ctr;
    private string triggerType; //The event that will enable the animation
    private int trigger; //A trigger can either be a key (cast SDL_Scancode to int) or a button
    
    public Animator(List<string> spritePathList, string triggerType, int trigger, int frameDelay) {
        this.spritePathList = spritePathList;
        currentSpritePath = spritePathList[0];
        this.triggerType = triggerType;
        this.trigger = trigger;
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

    public string nextSprite() {
        int index = spritePathList.IndexOf(currentSpritePath);
        int nextIndex = (index + 1) % (spritePathList.Count);
        string nextSprite = spritePathList[nextIndex];

        //If we have a frame delay, only switch when the counter reaches the amt of delay
        if (ctr >= frameDelay) {
            ctr = 0;
            currentSpritePath = nextSprite;
            return nextSprite;
        }

        ctr++;
        return currentSpritePath;
    }

    public void handleInput(InputEvent inp, string eventType) {
        Debug.Log("[LISA] INP: " + inp.Classification + " EVENTTYPE: " + eventType);
        enabled = false;
        if (eventType == triggerType) {
            if (eventType == "MouseDown" || eventType == "MouseUp" || eventType == "MouseMovement") { //In case of mouse event
                if (inp.Button == trigger) {
                    Enabled = true;
                }
            }
            else if (eventType == "KeyDown" || eventType == "KeyUp") { //In case of key event
                if (inp.Key == trigger) {
                    Enabled = true;
                }
            }
        }
    }
}