/*
*
*   The physics body class does... a lot.  It handles the computation of internal values such as 
*       the min and max values for X and Y (used by the Sweep and Prune algorithm, as well as 
*       collision detection in general).  It registers and processes the colliders that belong to 
*       an object.  It handles the application of forces and torque as well as drag and angular drag.
*       It lets an object add colliders, and then exposes those colliders for narrow phase collision 
*       detection.  It handles some naive default collision responses such as a simple reflection
*       or 'stop on collision'.
*       
*   Important to note though that while this is called a PhysicsBody, no claims are made for the 
*       *accuracy* of the physics.  If you are planning to do anything that requires the physics
*       calculations to be remotely correct, you're going to have to extend the engine so it does 
*       that.  All I'm interested in here is showing you how it's *architected*. 
*       
*   This is also the subsystem which I am least confident about people relying on, because it is 
*       virtually untestable in any meaningful sense.  I spent three days trying to track down a 
*       bug that mean that an object would pass through another one at a rate of approximately
*       once every half hour...
*       
*   @author Michael Heron
*   @version 1.0
*   
*   Several substantial contributions to the code made by others:
*   @author Mårten Åsberg (see Changelog for 1.0.1)
*   
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Shard
{
    class PhysicsBody
    {
        List<Collider> myColliders;
        List<Collider> collisionCandidates;
        GameObject parent;
        CollisionHandler colh;
        Transform trans;
        private float angularDrag;
        private float drag;
        private float torque;
        private float angularVelocity;
        private Vector2 force;
        private Vector2 velocity;
        private float mass;
        private double timeInterval;
        private float maxForce, maxTorque;
        private bool stopOnCollision;
        private bool reflectOnCollision;
        private bool impartForce;
        private bool passThrough;
        private bool usesGravity;
        private bool repelBodies;
        private double collisionCooldown = 0;
        private OnEdgeCollision edgeCollision = OnEdgeCollision.Nothing;
        private Color debugColor;
        public Color DebugColor { get => debugColor; set => debugColor = value; }

        private float[] minAndMaxX;
        private float[] minAndMaxY;

        public void applyGravity(float modifier, Vector2 dir)
        {

            Vector2 gf = dir * modifier;

            addForce(gf);

        }

        public void onColliders(Action<Collider> action)
        {
            foreach (var myCollider in myColliders)
            {
                action.Invoke(myCollider);
            }
        }

        public float AngularDrag { get => angularDrag; set => angularDrag = value; }
        public float Drag { get => drag; set => drag = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public float AngularVelocity { get => angularVelocity; set => angularVelocity = value; }
        internal GameObject Parent { get => parent; set => parent = value; }
        internal Transform Trans { get => trans; set => trans = value; }
        public float Mass { get => mass; set => mass = value; }
        public float[] MinAndMaxX { get => minAndMaxX; set => minAndMaxX = value; }
        public float[] MinAndMaxY { get => minAndMaxY; set => minAndMaxY = value; }
        public float MaxForce { get => maxForce; set => maxForce = value; }
        public float MaxTorque { get => maxTorque; set => maxTorque = value; }
        
        public double CollisionCooldown { get => collisionCooldown ; set => collisionCooldown = value; }
        
        public OnEdgeCollision EdgeCollision { get => edgeCollision; set => edgeCollision = value; }
        public bool PassThrough { get => passThrough || CollisionCooldown > 0; set => passThrough = value; }
        public bool UsesGravity { get => usesGravity; set => usesGravity = value; }
        public bool StopOnCollision { get => stopOnCollision; set => stopOnCollision = value; }
        public bool ReflectOnCollision { get => reflectOnCollision; set => reflectOnCollision = value; }
        public bool ImpartForce { get => this.impartForce; set => this.impartForce = value; }
        public bool RepelBodies { get => repelBodies; set => repelBodies = value; }
        internal CollisionHandler Colh { get => colh; set => colh = value; }

        public void drawMe()
        {
            foreach (Collider col in myColliders)
            {
                col.drawMe(DebugColor);
            }
        }

        public float[] getMinAndMax(bool x)
        {
            float min = Int32.MaxValue;
            float max = -1 * min;
            float[] tmp;

            foreach (Collider col in myColliders)
            {

                if (x)
                {
                    tmp = col.MinAndMaxX;
                }
                else
                {
                    tmp = col.MinAndMaxY;
                }


                if (tmp[0] < min)
                {
                    min = tmp[0];
                }

                if (tmp[1] > max)
                {
                    max = tmp[1];
                }
            }


            return new float[2] { min, max };
        }

        public PhysicsBody(GameObject p)
        {
            DebugColor = Color.Green;

            myColliders = new List<Collider>();
            collisionCandidates = new List<Collider>();

            Parent = p;
            Trans = p.Transform;
            Colh = (CollisionHandler)p;

            AngularDrag = 0.01f;
            Drag = 0.01f;
            Drag = 0.01f;
            Mass = 1;
            MaxForce = 10;
            MaxTorque = 2;
            usesGravity = false;
            stopOnCollision = true;
            reflectOnCollision = false;

            MinAndMaxX = new float[2];
            MinAndMaxY = new float[2];

            timeInterval = PhysicsManager.getInstance().TimeInterval;
            //            Debug.getInstance().log ("Setting physics enabled");

            PhysicsManager.getInstance().addPhysicsObject(this);
        }

        public void addTorque(float dir)
        {

            torque += dir / Mass;

            if (torque > MaxTorque)
            {
                torque = MaxTorque;
            }

            if (torque < -1 * MaxTorque)
            {
                torque = -1 * MaxTorque;
            }


        }

        public void reverseForces(float prop)
        {
            force *= -prop;
        }

        public void impartForces(PhysicsBody other, float massProp)
        {
            other.addForce(velocity * massProp);

            recalculateColliders();

        }

        public void stopForces()
        {
            force = Vector2.Zero;
            velocity = Vector2.Zero;
        }

        public void reflectForces(Vector2 impulse)
        {
            Vector2 reflect = new Vector2(0, 0);

            Debug.Log ("Reflecting " + impulse);

            // We're being pushed to the right, so we must have collided with the right.
            if (impulse.X > 0)
            {
                reflect.X = -1;
            }

            // We're being pushed to the left, so we must have collided with the left.
            if (impulse.X < 0)
            {
                reflect.X = -1;

            }

            // We're being pushed upwards, so we must have collided with the top.
            if (impulse.Y < 0)
            {
                reflect.Y = -1;
            }

            // We're being pushed downwards, so we must have collided with the bottom.
            if (impulse.Y > 0)
            {
                reflect.Y = -1;

            }


            force *= reflect;

            Debug.Log("Reflect is " + reflect);

        }

        public void reduceForces(float prop) {
            velocity *= prop;
        }

        public void addForce(Vector2 dir, float force) {
            addForce(dir * force);
        }

        public void addForce(Vector2 dir)
        {
            dir /= Mass;

            // Set a lower bound.
            if (dir.LengthSquared() < 0.0001)
            {
                return;
            }

            force += dir;

            // Set a higher bound.
            if (force.Length() > MaxForce)
            {
                force = Vector2.Normalize(force) * MaxForce;
            }
        }

        public void recalculateColliders()
        {
            foreach (Collider col in getColliders())
            {
                col.recalculate();
            }

            MinAndMaxX = getMinAndMax(true);
            MinAndMaxY = getMinAndMax(false);
        }

        private void updateVelocities(float timeModifier)
        {
            
            float instantDrag = Drag * timeModifier;
            float instantAngularDrag = angularDrag * timeModifier;
            
            if(force.Length() != 0)
            {
                velocity += force * timeModifier;
            }

            float velocityMagnitude = velocity.Length();
            
            if (velocityMagnitude < instantDrag)
            {
                velocity = Vector2.Zero;
            }
            else if(instantDrag > 0)
            {
                velocity -= velocity * instantDrag;
            }
            

            if(torque != 0)
                angularVelocity += torque * timeModifier;
            if ( Math.Abs(angularVelocity) < instantAngularDrag)
            {
                angularVelocity = 0;
            }
            else
            {
                angularVelocity -= instantAngularDrag * angularVelocity;
            }
        }

        private List<Collider.Bound> anyCollide()
        {
            int width = DisplayText.WIDTH;
            int height = DisplayText.HEIGHT;
            return myColliders.Aggregate(new List<Collider.Bound>(),
                (a, b) => a.Union(b.isOutOfBounds(width, height)).ToList());
        }

        private bool isXEscaping(List<Collider.Bound> bound)
        {
            return (bound.Contains(Collider.Bound.Left) && velocity.X < 0) ||
                   (bound.Contains(Collider.Bound.Right) && velocity.X > 0);
        }

        private bool isYEscaping(List<Collider.Bound> bound)
        {
            return (bound.Contains(Collider.Bound.Bottom) && velocity.Y > 0) ||
                   (bound.Contains(Collider.Bound.Top) && velocity.Y < 0);
        }

        private void handleScreenEdgeCollisions()
        {
            int width = Bootstrap.getDisplay().getWidth();
            int height = Bootstrap.getDisplay().getHeight();
            if (edgeCollision == OnEdgeCollision.Nothing)
                return;
            
            List<Collider.Bound> bound = anyCollide();
            if (bound.Count == 0)
                return;

            bool isXEscaping = this.isXEscaping(bound);
            bool isYEscaping = this.isYEscaping(bound);
            
            switch (edgeCollision)
            {
                case OnEdgeCollision.Rebound:
                    if ( isXEscaping)
                    {
                        velocity.X = -velocity.X;
                    }
                    if (isYEscaping)
                    {
                        velocity.Y = -velocity.Y;
                    }
                    break;
                
                case OnEdgeCollision.MarkForDestruction:
                    if (isXEscaping || isYEscaping)
                        parent.ToBeDestroyed = true;
                    break;
                
                case OnEdgeCollision.Stop:
                    if (isXEscaping)
                    {
                        velocity.X = 0;
                    }

                    if (isYEscaping)
                    {
                        velocity.Y = 0;
                    }
                    break;
            }
        }
        
        private void handleTranslations(float timeModifier)
        {
            handleScreenEdgeCollisions();
            trans.translate(velocity * timeModifier);
        }
        
        public void physicsTick(float timeSinceLastUpdate)
        {
            float timeModifier = timeSinceLastUpdate / 20;

            updateVelocities(timeModifier);
            collisionCooldown -= timeSinceLastUpdate;
            if(collisionCooldown <= 0)
                collisionCooldown = 0;
            trans.rotate(angularVelocity * timeModifier);
			handleTranslations(timeModifier);

            this.force = Vector2.Zero;
            this.torque = 0;

        }


        public ColliderRect addRectCollider()
        {
            ColliderRect cr = new ColliderRect((CollisionHandler)parent, parent.Transform);

            addCollider(cr);

            return cr;
        }

        public ColliderCircle addCircleCollider()
        {
            ColliderCircle cr = new ColliderCircle((CollisionHandler)parent, parent.Transform);

            addCollider(cr);

            return cr;
        }

        public ColliderCircle addCircleCollider(int x, int y, int rad)
        {
            ColliderCircle cr = new ColliderCircle((CollisionHandler)parent, parent.Transform, x, y, rad);

            addCollider(cr);

            return cr;
        }


        public ColliderRect addRectCollider(int x, int y, int wid, int ht)
        {
            ColliderRect cr = new ColliderRect((CollisionHandler)parent, parent.Transform, x, y, wid, ht);

            addCollider(cr);

            return cr;
        }


        public void addCollider(Collider col)
        {
            myColliders.Add(col);
        }

        public List<Collider> getColliders()
        {
            return myColliders;
        }

        public Vector2? checkCollisions(Vector2 other)
        {
            Vector2? d;


            foreach (Collider c in myColliders)
            {
                d = c.checkCollision(other).Item1;

                if (d != null)
                {
                    return d;
                }
            }

            return null;
        }


        public Vector2? checkCollisions(Collider other)
        {
            Vector2? d;

//            Debug.Log("Checking collision with " + other);
            foreach (Collider c in myColliders)
            {
                d = c.checkCollision(other).Item1;

                if (d != null)
                {
                    return d;
                }
            }

            return null;
        }
    }
}