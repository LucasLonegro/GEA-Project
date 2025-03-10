using SDL2;
using Shard;
using System.Drawing;

namespace GameTest {
    class Spaceship : GameObject, InputListener, CollisionHandler {
        bool up, down, turnLeft, turnRight;
        bool up2, down2, turnLeft2, turnRight2;

        bool isPlayer1Controlled, isPlayer2Controlled;

        
        // Constructor to determine player control
        public Spaceship(bool isPlayer1) {
            isPlayer1Controlled = isPlayer1;
            isPlayer2Controlled = !isPlayer1;
        }
        
        public override void initialize() {
            Transform.X = 500.0f;
            Transform.Y = 500.0f;
            
            //Set default sprite
            Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("spaceship.png");

            //Animation test
            setAnimationEnabled();

            Transform.addAnimation("go", [
                Bootstrap.getAssetManager().getAssetPath("spaceship.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship2.png"),
                Bootstrap.getAssetManager().getAssetPath("spaceship3.png")
            ], 30);
            
            
            Transform.addAnimation("nogo", [
                Bootstrap.getAssetManager().getAssetPath("spaceship3.png")
            ]);


            Bootstrap.getInput().addListener(this);
            
            up = down = turnLeft = turnRight = false;
            up2 = down2 = turnLeft2 = turnRight2 = false;

            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 10;
            MyBody.AngularDrag = 0.1f;
            MyBody.Drag = 0.05f;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.ImpartForce = true;
            MyBody.RepelBodies = true;


            //           MyBody.PassThrough = true;
            //            MyBody.addCircleCollider(0, 0, 5);
            //            MyBody.addCircleCollider(0, 34, 5);
            //            MyBody.addCircleCollider(60, 18, 5);
            //     MyBody.addCircleCollider();

            MyBody.addRectCollider();

            addTag("Spaceship");
        }

        public void fireBullet() {
            Bullet b = new Bullet();

            b.setupBullet(this, this.Transform.Centre.X, this.Transform.Centre.Y);

            b.Transform.rotate(this.Transform.Rotz);

            Bootstrap.getSound().playSound("fire.wav");
        }

        public void handleInput(InputEvent inp, string eventType) {
            if (eventType == "KeyDown") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W) {
                    up = true;
                    if (isPlayer1Controlled) {
                        Transform.enableAnimation("go"); 
                    }
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_S) {
                    down = true;
                    if (isPlayer1Controlled) {
                        Transform.enableAnimation("nogo");
                    }
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D) turnRight = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A) turnLeft = true;
                
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_UP) up2 = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN) down2 = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT) turnRight2 = true;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT) turnLeft2 = true;
            }
            else if (eventType == "KeyUp") {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W) {
                    up = false;
                    Transform.disableAnimation();
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_S) {
                    down = false;
                    Transform.disableAnimation();
                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D) turnRight = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A) turnLeft = false;
                
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_UP) up2 = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN) down2 = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT) turnRight2 = false;
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT) turnLeft2 = false;

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE && isPlayer1Controlled) fireBullet();
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_RCTRL && isPlayer2Controlled) fireBullet();
            }
        }

        public override void physicsUpdate() {
            if (isPlayer1Controlled) {
                if (turnLeft) MyBody.addTorque(-0.6f);
                if (turnRight) MyBody.addTorque(0.6f);
                if (up) MyBody.addForce(this.Transform.Forward, 0.5f);
                if (down) MyBody.addForce(this.Transform.Forward, -0.2f);
            }
            else if (isPlayer2Controlled) {
                if (turnLeft2) MyBody.addTorque(-0.6f);
                if (turnRight2) MyBody.addTorque(0.6f);
                if (up2) MyBody.addForce(this.Transform.Forward, 0.5f);
                if (down2) MyBody.addForce(this.Transform.Forward, -0.2f);
            }
        }

        public override void update() {
            Bootstrap.getDisplay().addToDraw(this);
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