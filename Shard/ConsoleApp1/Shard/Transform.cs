/*
 *
 *   The transform class handles position, independent of physics and forces (although the physics
 *       system will make use of the rotation and translation functions here).  Essentially this class
 *       is a game object's location (X, Y), rotation and scale.  Usefully it also calculates the
 *       centre of an object as well as relative directions such as forwards and right.  If you want
 *       backwards and left, multiply forward or right by -1.
 *
 *   @author Michael Heron
 *   @version 1.0
 *
 */


using System;
using System.Collections.Generic;
using System.Numerics;

namespace Shard {
    class Transform {
        private GameObject owner;
        private float x, y;
        private float lx, ly;
        private float rotz;
        private int wid, ht;
        private float scalex, scaley;
        private string spritePath;
        private Vector2 forward;
        private Vector2 right, centre;
        private Dictionary<string, Animator> animatorDict;

        public Vector2 getLastDirection() {
            float dx, dy;
            dx = (X - Lx);
            dy = (Y - Ly);

            return new Vector2(-dx, -dy);
        }

        public Transform(GameObject ow) {
            Owner = ow;
            forward = new Vector2();
            right = new Vector2();
            centre = new Vector2();

            scalex = 1.0f;
            scaley = 1.0f;

            x = 0;
            y = 0;

            lx = 0;
            ly = 0;

            rotate(0);
        }


        public void recalculateCentre() {
            centre.X = (float)(x + ((this.Wid * scalex) / 2));
            centre.Y = (float)(y + ((this.Ht * scaley) / 2));
        }

        public void translate(double nx, double ny) {
            translate((float)nx, (float)ny);
        }


        public void translate(float nx, float ny) {
            Lx = X;
            Ly = Y;

            x += (float)nx;
            y += (float)ny;


            recalculateCentre();
        }

        public void translate(Vector2 vect) {
            translate(vect.X, vect.Y);
        }


        public void rotate(float dir) {
            rotz += (float)dir;

            rotz %= 360;

            float angle = (float)(Math.PI * rotz / 180.0f);
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            forward.X = cos;
            forward.Y = sin;


            right.X = -1 * forward.Y;
            right.Y = forward.X;
        }


        public float X {
            get => x;
            set => x = value;
        }

        public float Y {
            get => y;
            set => y = value;
        }

        public float Rotz {
            get => rotz;
            set => rotz = value;
        }

        public Dictionary<string, Animator> AnimatorDict {
            get => animatorDict;
            set => animatorDict = value;
        }

        /**
         * Creates an animator with spritepaths for a specific animation
         */
        public void addAnimation(String name, List<string> spritePathList) {
            if (animatorDict != null) {
                Animator anim = new Animator();
                anim.SpritePathList = spritePathList;
                anim.CurrentSpritePath = spritePathList[0];
                animatorDict.Add(name, anim);
            }
        }
        
        /**
         * Overloaded method. Can add the framedelay when creating animation
         */
        public void addAnimation(String name, List<string> spritePathList, int frameDelay) {
            if (animatorDict != null) {
                Animator anim = new Animator();
                anim.SpritePathList = spritePathList;
                anim.CurrentSpritePath = spritePathList[0];
                anim.FrameDelay = frameDelay;
                animatorDict.Add(name, anim);
            }
        }

        public void setAnimationFrameDelay(String name, int frameDelay) {
            if (animatorDict != null) {
                Animator anim = animatorDict[name]; //TODO: ISSUE HEREEEE~!!!!!
                anim.FrameDelay = frameDelay;
            }
        }

        public string SpritePath {
            get => spritePath;
            set => spritePath = value;
        }

        /**
         * Enables a specific animation using its name
         */
        public void enableAnimation(string name) {
            //If there is at least 1 animation present...
            if (animatorDict != null) { 
                foreach (KeyValuePair<string, Animator> elem in animatorDict) {
                    elem.Value.Enabled = false; //Set all to disabled
                    
                    //EXCEPT the animation we want to enable
                    if (elem.Key == name) { //TODO: can be cleaner?
                        elem.Value.Enabled = true;
                    }
                }
            }
        }

        /**
         * Disables all animations, so the spritepath will go back to default
         */
        public void disableAnimation() {
            if (animatorDict != null) { //If there is at least 1 animation present
                foreach (KeyValuePair<string, Animator> elem in animatorDict) {
                    elem.Value.Enabled = false; //Set all to disabled
                }
            }
        }

        /**
         * Method that returns the next spritepath in case one of the animations is enabled
         */
        public string getSpritePathUpdated() {
            if (animatorDict != null) {
                foreach (Animator anim in animatorDict.Values) {
                    if (anim.Enabled) {
                        return anim.nextSprite();
                    }
                }
            }
            return spritePath;
        }

        public ref Vector2 Forward {
            get => ref forward;
        }

        public int Wid {
            get => wid;
            set => wid = value;
        }

        public int Ht {
            get => ht;
            set => ht = value;
        }

        public ref Vector2 Right {
            get => ref right;
        }

        internal GameObject Owner {
            get => owner;
            set => owner = value;
        }

        public ref Vector2 Centre {
            get => ref centre;
        }

        public float Scalex {
            get => scalex;
            set => scalex = value;
        }

        public float Scaley {
            get => scaley;
            set => scaley = value;
        }

        public float Lx {
            get => lx;
            set => lx = value;
        }

        public float Ly {
            get => ly;
            set => ly = value;
        }
    }
}