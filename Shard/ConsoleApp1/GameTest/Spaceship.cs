using SDL2;
using Shard;
using System.Drawing;

namespace GameTest {
    class Spaceship : GameObject, InputListener, CollisionHandler {
        bool up, down, turnLeft, turnRight;

        private int upKey, downKey, leftKey, rightKey, shootKey;

        private static float fireDelay = 0.05f;
        private float fireCooldown = 0;
        private float thrust = 0.6f;
        private float backThrust = 0.2f;
        private float torque = 1.2f;


        // Constructor to determine player control
        public Spaceship(bool isPlayer1) {
            if (isPlayer1) {
                upKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_W;
                downKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_S;
                leftKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_A;
                rightKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_D;
                shootKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE;
            }
            else {
                upKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_UP;
                downKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN;
                leftKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT;
                rightKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT;
                shootKey = (int)SDL.SDL_Scancode.SDL_SCANCODE_RCTRL;
            }
        }

        public override void initialize() {

            Bootstrap.getInput().addListener(this);

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

        public void fireBullet() {
            if (fireCooldown < fireDelay) {
                return;
            }

            fireCooldown = 0;

            Bullet b = new Bullet();

            b.setupBullet(this, this.Transform.Centre.X, this.Transform.Centre.Y);

            b.Transform.rotate(this.Transform.Rotz);

            Bootstrap.getSound().playSound("fire.wav");
        }

        public void handleInput(InputEvent inp, string eventType) {
            if (eventType == "KeyDown") {
                if (inp.Key == upKey) {
                    up = true;
                    Transform.enableAnimation("go");
                }
                if (inp.Key == downKey) down = true;

                if (inp.Key == rightKey) turnRight = true;
                if (inp.Key == leftKey) turnLeft = true;
            }
            else if (eventType == "KeyUp") {
                if (inp.Key == upKey) {
                    up = false;
                    Transform.disableAnimation();
                }
                if (inp.Key == downKey) down = false;

                if (inp.Key == rightKey) turnRight = false;
                if (inp.Key == leftKey) turnLeft = false;

                if (inp.Key == shootKey) fireBullet();
            }
        }

        public override void physicsUpdate() {
            if (turnLeft) MyBody.addTorque(-torque);
            if (turnRight) MyBody.addTorque(torque);
            if (up) MyBody.addForce(Transform.Forward, thrust);
            if (down) MyBody.addForce(Transform.Forward, -backThrust);
        }

        public override void update() {
            Bootstrap.getDisplay().addToDraw(this);
            fireCooldown += (float)Bootstrap.getDeltaTime();
        }

        public void onCollisionEnter(PhysicsBody x) {
            if (x.Parent.checkTag("Bullet") == false) {
                MyBody.DebugColor = Color.Red;
            }
        }

        public void onCollisionExit(PhysicsBody x) {
            MyBody.DebugColor = Color.Green;
        }

        public void onCollisionStay(PhysicsBody x) {
            MyBody.DebugColor = Color.Blue;
        }

        public override string ToString() {
            return "Spaceship: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " +
                   Transform.Ht + "]";
        }
    }
}