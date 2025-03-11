using Shard;

namespace GameTest {
    class Goalpost : GameObject {
        bool isLeftPost, isRightPost;

        
        public Goalpost(bool isLeft)
        {
            isLeftPost = isLeft;
            isRightPost = !isLeft;
            
            if(isLeft){
                Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("LeftGoalpost.png");
            }
            else{
                Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("RightGoalpost.png");
            }
        }
        public override void initialize() {
            // Add physics if needed
            // setPhysicsEnabled();
            // MyBody.Mass = 1;
            // MyBody.StopOnCollision = true; // Prevents passing through
            // MyBody.addRectCollider(); // If you want collisions

            addTag("Goalpost");
        }

        public override void update() {
            Bootstrap.getDisplay().addToDraw(this); // Render the goalpost
        }
    }
}