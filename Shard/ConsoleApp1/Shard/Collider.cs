/*
*
*   The abstract collider class that is the base of all collisions.   Specific variations should
*       extend from this or one of its children.
*   @author Michael Heron
*   @version 1.0
*   
*/

using System;
using System.Drawing;
using System.Numerics;
using System.Collections.Generic;

namespace Shard
{
    abstract class Collider
    {
        private CollisionHandler gameObject;
        private float[] minAndMaxX;
        private float[] minAndMaxY;
        private bool rotateAtOffset;
        private List<CollisionHandler> ignoreObjects;

        public abstract void recalculate();
        public Collider(CollisionHandler gob)
        {
            gameObject = gob;
            MinAndMaxX = new float[2];
            MinAndMaxY = new float[2];
            ignoreObjects = new List<CollisionHandler>();

        }

        public enum Bound
        {
            Top,
            Bottom,
            Left,
            Right,
        }

        internal CollisionHandler GameObject { get => gameObject; set => gameObject = value; }
        internal float[] MinAndMaxX { get => minAndMaxX; set => minAndMaxX = value; }
        internal float[] MinAndMaxY { get => minAndMaxY; set => minAndMaxY = value; }
        public bool RotateAtOffset { get => rotateAtOffset; set => rotateAtOffset = value; }
        
        public abstract Bound? isOutOfBounds(int width, int height);

        public abstract (Vector2?, double?) checkCollision(ColliderRect c);

        public abstract (Vector2?, double?) checkCollision(Vector2 c);

        public abstract (Vector2?, double?) checkCollision(ColliderCircle c);

        public virtual (Vector2?, double?) checkCollision(Collider c)
        {
            
            if(ignoreObjects.Contains(c.gameObject) || c.ignoreObjects.Contains(gameObject))
                return (null, null);

            if (c is ColliderRect)
            {
                return checkCollision((ColliderRect)c);
            }

            if (c is ColliderCircle)
            {
                return checkCollision((ColliderCircle)c);
            }

            Debug.getInstance().log("Bug");
            // Not sure how we got here but c'est la vie
            return (null,null);
        }

        public void ignoreCollider(CollisionHandler o)
        {
            ignoreObjects.Add(o);
        }

        public abstract void drawMe(Color col);

        public abstract float[] getMinAndMaxX();
        public abstract float[] getMinAndMaxY();

    }
}
