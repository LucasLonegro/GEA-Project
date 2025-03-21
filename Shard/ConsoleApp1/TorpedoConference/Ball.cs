﻿using System;
using Shard;
using System.Numerics;

namespace GameTest
{
    class Ball : GameObject, CollisionHandler, InputListener
    {
        int torqueCounter = 0;
        private TorpedoConference game;
        public void handleInput(InputEvent inp, string eventType)
        {

            if (eventType == "MouseDown" && inp.Button == 2)
            {
                if (MyBody.checkCollisions(new Vector2(inp.X, inp.Y)) != null)
                {
                    torqueCounter += 10;
                }
            }
        }

        public override void initialize()
        {
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("asteroid.png");

            setPhysicsEnabled();

            MyBody.MaxTorque = 100;
            MyBody.Mass = 1;
            MyBody.AngularDrag = 0.0f;
            MyBody.Drag = 0.05f;
            MyBody.MaxForce = 100;
            MyBody.Velocity = this.Transform.Right * 2.5f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = true;
            MyBody.RepelBodies = true;
            MyBody.EdgeCollision = OnEdgeCollision.Rebound;

            //            MyBody.addCircleCollider(32, 32, 30);
            MyBody.addCircleCollider();
            Bootstrap.getInput().addListener(this);

            addTag("Ball");

        }

        public Ball(TorpedoConference game)
        {
            this.game = game;
        }


        public override void physicsUpdate()
        {
            for (int i = 0; i < torqueCounter; i++)
            {
                MyBody.addTorque(1.0f);
            }

            if (torqueCounter > 0)
            {
                torqueCounter -= 1;
            }
            


        }

        public override void update()
        {
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Goalpost")) //TODO: NOT OOP! DISGUSTING!
            {
                game.goal(this);
            }

        }

        public void onCollisionExit(PhysicsBody x)
        {
            Debug.getInstance().log("Anti Bang");
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "Asteroid: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }
    }
}
