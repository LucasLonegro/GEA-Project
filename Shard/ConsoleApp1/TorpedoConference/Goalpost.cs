using System.Drawing;
using Shard;

namespace GameTest {
    class Goalpost : GameObject, CollisionHandler {

        private int goalsScored = 0;

        public int GoalsScored {
           get => goalsScored; 
        }
        
        public override void initialize() {
            // Add physics if needed
            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.StopOnCollision = true;
            MyBody.ReflectOnCollision = false;
            MyBody.RepelBodies = false;
            MyBody.MaxForce = 0.0f;
            
            MyBody.addRectCollider();

            addTag("Goalpost");
        }

        public override void update() {
            Bootstrap.getDisplay().addToDraw(this); // Render the goalpost
        }
        
        public void onCollisionEnter(PhysicsBody x)
        {
            MyBody.DebugColor = Color.Red;
            if (x.Parent.checkTag("Ball")) {
                Debug.Log("SCORE");
                goalsScored++; //TODO: this could be cleaner, have the game be a listener for collisions etc etc
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
        
    }
}