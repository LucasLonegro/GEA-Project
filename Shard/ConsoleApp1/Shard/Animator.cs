using System.Collections.Generic;

namespace Shard;

public class Animator : InputListener {
    private List<string> spritePathList;
    private string currentSpritePath;
    private bool enabled;
    private int frameDelay;
    private int ctr;
    private string triggerEvent; //The event that will enable the animation
    private int trigger; //A trigger can either be a key (cast SDL_Scancode to int) or a button
    
    public Animator(List<string> spritePathList, string triggerEvent, int trigger, int frameDelay) {
        this.spritePathList = spritePathList;
        currentSpritePath = spritePathList[0];
        this.triggerEvent = triggerEvent;
        this.trigger = trigger;
        this.frameDelay = frameDelay;
    }
    
    public bool Enabled {
        get => enabled;
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
        enabled = false;
        
        if (eventType == triggerEvent) {
            if (eventType == "MouseDown" || eventType == "MouseUp" || eventType == "MouseMovement") { //In case of mouse event
                if (inp.Button == trigger) {
                    enabled = true;
                }
            }
            else if (eventType == "KeyDown" || eventType == "KeyUp") { //In case of key event
                if (inp.Key == trigger) {
                    enabled = true;
                }
            }
        }
    }
}