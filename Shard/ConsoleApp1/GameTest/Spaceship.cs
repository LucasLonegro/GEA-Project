using System;
using System.Collections.Generic;
using SDL2;
using Shard;
using System.Drawing;

namespace GameTest
{
    class Spaceship : GameObject, InputListener, CollisionHandler
    {
        bool up, down, turnLeft, turnRight;

        bool isPlayer1Controlled, isPlayer2Controlled;

        private static float fireDelay = 0.05f;
        private float fireCooldown = 0;
        private float thrust = 0.6f;
        private float backThrust = 0.2f;
        private float torque = 1.2f;


        // Constructor to determine player control
        public Spaceship(bool isPlayer1)
        {
            isPlayer1Controlled = isPlayer1;
            isPlayer2Controlled = !isPlayer1;

            // Assign animations based on which player this spaceship belongs to
            if (isPlayer1Controlled)
            {
                Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("spaceshipA.png");
            }
            else
            {
                Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("spaceshipB.png");
            }
            
            InputSystem il = Bootstrap.getInput();
            
            if (!isPlayer1Controlled)
            {
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_W, 0);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_S, 0);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_D, 0);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_A, 0);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE, 0);
                
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_UP, (int)SDL.SDL_Scancode.SDL_SCANCODE_W);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN, (int)SDL.SDL_Scancode.SDL_SCANCODE_S);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT, (int)SDL.SDL_Scancode.SDL_SCANCODE_D);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT, (int)SDL.SDL_Scancode.SDL_SCANCODE_A);
                il.setMapping(this, (int)SDL.SDL_Scancode.SDL_SCANCODE_RCTRL, (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE);
            }
        }

        public override void initialize()
        {
            // this.Transform.X = 500.0f;
            // this.Transform.Y = 500.0f;

            //Animation test
            setAnimationEnabled();

            InputSystem il = Bootstrap.getInput();
            il.addListener(this);

            Animator anim = new Animator([
                Bootstrap.getAssetManager().getAssetPath("spaceship.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship2.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship3.png")
            ], "KeyDown", (int)SDL.SDL_Scancode.SDL_SCANCODE_W, 30);
            addAnimation("go", anim);
            
            il.addListener(anim);

            up = down = turnLeft = turnRight = false;

            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 10;
            MyBody.AngularDrag = 0.2f;
            MyBody.Drag = 0.05f;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = true;
            MyBody.RepelBodies = true;
            MyBody.EdgeCollision = OnEdgeCollision.Stop;


            //           MyBody.PassThrough = true;
            //            MyBody.addCircleCollider(0, 0, 5);
            //            MyBody.addCircleCollider(0, 34, 5);
            //            MyBody.addCircleCollider(60, 18, 5);
            //     MyBody.addCircleCollider();

            MyBody.addRectCollider();

            addTag("Spaceship");
        }

        public void fireBullet()
        {
            if (fireCooldown < fireDelay)
            {
                return;
            }

            fireCooldown = 0;

            Bullet b = new Bullet();

            b.setupBullet(this, this.Transform.Centre.X, this.Transform.Centre.Y);

            b.Transform.rotate(this.Transform.Rotz);

            Bootstrap.getSound().playSound("fire.wav");
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "KeyDown")
            {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W) up = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_S) down = true;

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D) turnRight = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A) turnLeft = true;

            }
            else if (eventType == "KeyUp")
            {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W) up = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_S) down = false;

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D) turnRight = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A) turnLeft = false;

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE) fireBullet();
            }
        }

        public override void physicsUpdate()
        {
            if (turnLeft) MyBody.addTorque(-torque);
            if (turnRight) MyBody.addTorque(torque);
            if (up) MyBody.addForce(this.Transform.Forward, thrust);
            if (down) MyBody.addForce(this.Transform.Forward, -backThrust);
        }

        public override void update()
        {
            Bootstrap.getDisplay().addToDraw(this);
            fireCooldown += (float)Bootstrap.getDeltaTime();
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Bullet") == false)
            {
                MyBody.DebugColor = Color.Red;
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {
            MyBody.DebugColor = Color.Green;
        }

        public void onCollisionStay(PhysicsBody x)
        {
            MyBody.DebugColor = Color.Blue;
        }

        public override string ToString()
        {
            return "Spaceship: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " +
                   Transform.Ht + "]";
        }
    }
}