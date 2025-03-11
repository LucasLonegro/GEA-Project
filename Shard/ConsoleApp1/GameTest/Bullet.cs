using Shard;
using System;
using System.Drawing;
using System.Numerics;

namespace GameTest
{
    class Bullet : GameObject, CollisionHandler
    {
        private Spaceship origin;

        public void setupBullet(Spaceship or, float x, float y)
        {
            this.Transform.X = x;
            this.Transform.Y = y;
            this.Transform.Wid = 10;
            this.Transform.Ht = 10;

            this.origin = or;

            setPhysicsEnabled();

            MyBody.addRectCollider((int)x, (int)y, 10, 10);

            addTag("Bullet");

            //            MyBody.addCircleCollider((int)x, (int)y, 5);

            MyBody.Mass = 0.1f;
            MyBody.MaxForce = 50;
            MyBody.Velocity = or.Transform.Forward * 10.0f;
            MyBody.Drag = 0.0f;

            MyBody.PassThrough = false;
            MyBody.ImpartForce = true;
            MyBody.RepelBodies = true;
            MyBody.ReflectOnCollision = false;
            MyBody.EdgeCollision = OnEdgeCollision.MarkForDestruction;
            MyBody.onColliders(c => c.ignoreCollider(or));
        }

        public override void initialize()
        {
            this.Transient = true;
        }

        public override void physicsUpdate()
        {
        }

        public override void update()
        {
            Random r = new Random();
            Color col = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), 0);


            Bootstrap.getDisplay().drawLine(
                (int)Transform.X,
                (int)Transform.Y,
                (int)Transform.X + 10,
                (int)Transform.Y + 10,
                col);

            Bootstrap.getDisplay().drawLine(
                (int)Transform.X + 10,
                (int)Transform.Y,
                (int)Transform.X,
                (int)Transform.Y + 10,
                col);


        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Spaceship") == false && false)
            {
                Debug.Log("Boom! " + x);
                ToBeDestroyed = true;
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "Bullet: " + Transform.X + ", " + Transform.X;
        }
    }
}
