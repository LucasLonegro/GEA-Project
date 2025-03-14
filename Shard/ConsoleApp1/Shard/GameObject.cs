﻿/*
 *
 *   Anything that is going to be an interactable object in your game should extend from GameObject.
 *       It handles the life-cycle of the objects, some useful general features (such as tags), and serves
 *       as the convenient facade to making the object work with the physics system.  It's a good class, Bront.
 *   @author Michael Heron
 *   @version 1.0
 *
 */

using System;
using System.Collections.Generic;

namespace Shard {
    class GameObject {
        private Transform3D transform;
        private bool transient;
        private bool toBeDestroyed;
        private bool visible;
        private PhysicsBody myBody;
        private List<string> tags;
        private Dictionary<string, Animator> animatorDict;

        public void addTag(string str) {
            if (tags.Contains(str)) {
                return;
            }

            tags.Add(str);
        }

        public void removeTag(string str) {
            tags.Remove(str);
        }

        public bool checkTag(string tag) {
            return tags.Contains(tag);
        }

        public String getTags() {
            string str = "";

            foreach (string s in tags) {
                str += s;
                str += ";";
            }

            return str;
        }

        /**
         * When setting the animation as enabled, the transform property will get an animator assigned.
         * This animator will then handle changes in spritepath
         */
        public void setAnimationEnabled() {
            animatorDict = new Dictionary<string, Animator>();
        }

        public Dictionary<string, Animator> AnimatorDict {
            get => animatorDict;
        }
        
        /**
         * Method that adds pre-existing animator to the dict
         */
        public void addAnimation(String name, Animator anim) {
            if (animatorDict != null) {
                animatorDict.Add(name, anim);
                Bootstrap.getInput().addListener(anim);
            }
        }
        
        /**
         * Method that creates the animator and adds it in one fell swoop
         */
        public void addAnimation(String name, List<string> spritePathList, string triggerType, int trigger, int frameDelay) {
            if (animatorDict != null) {
                Animator anim = new Animator(spritePathList, triggerType, trigger, frameDelay);
                animatorDict.Add(name, anim);
                Bootstrap.getInput().addListener(anim);
            }
        }

        public void setPhysicsEnabled() {
            MyBody = new PhysicsBody(this);
        }
        
        public bool queryPhysicsEnabled() {
            if (MyBody == null) {
                return false;
            }

            return true;
        }

        internal Transform3D Transform {
            get => transform;
        }

        internal Transform Transform2D {
            get => (Transform)transform;
        }


        public bool Visible {
            get => visible;
            set => visible = value;
        }

        public bool Transient {
            get => transient;
            set => transient = value;
        }

        public bool ToBeDestroyed {
            get => toBeDestroyed;
            set => toBeDestroyed = value;
        }

        internal PhysicsBody MyBody {
            get => myBody;
            set => myBody = value;
        }

        public virtual void initialize() {
        }

        public virtual void update() {
        }

        public virtual void physicsUpdate() {
        }

        public virtual void prePhysicsUpdate() {
        }

        public GameObject() {
            GameObjectManager.getInstance().addGameObject(this);

            transform = new Transform3D(this);
            visible = false;

            ToBeDestroyed = false;
            tags = new List<string>();

            this.initialize();
        }

        public void checkDestroyMe() {
            if (!transient) {
                return;
            }

            if (Transform.X > 0 && Transform.X < Bootstrap.getDisplay().getWidth()) {
                if (Transform.Y > 0 && Transform.Y < Bootstrap.getDisplay().getHeight()) {
                    return;
                }
            }


            ToBeDestroyed = true;
        }

        public virtual void killMe() {
            PhysicsManager.getInstance().removePhysicsObject(myBody);

            myBody = null;
            transform = null;
        }
    }
}