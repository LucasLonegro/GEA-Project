using System;

namespace Shard;

using System.Drawing;

class ScreenEdge : GameObject, InputListener, CollisionHandler
{
    public enum ScreenEdgeType
    {
        Top,
        Bottom,
        Left,
        Right,
    }

    public ScreenEdge(ScreenEdgeType edgeType)
    {
        setPhysicsEnabled();
        setAnimationEnabled();
        Bootstrap.getInput().addListener(this);
        addTag("Edge");
        MyBody.MaxForce = 0.0f; // make the edges immovable
        int height = Bootstrap.getDisplay().getHeight();
        int width = Bootstrap.getDisplay().getWidth();
        Console.WriteLine(height);
        Console.WriteLine(width);

        MyBody.RepelBodies = true;
        MyBody.ImpartForce = true;

        switch (edgeType)
        {
            case ScreenEdgeType.Top:
                this.Transform.Wid = width * 3;
                this.Transform.Ht = height;
                this.Transform.X = -width;
                this.Transform.Y = -height;
                break;
            case ScreenEdgeType.Bottom:
                this.Transform.Wid = width * 3;
                this.Transform.Ht = height;
                this.Transform.X = -width;
                this.Transform.Y = height;
                break;
            case ScreenEdgeType.Left:
                this.Transform.Wid = width;
                this.Transform.Ht = height;
                this.Transform.X = -width;
                this.Transform.Y = 0;
                break;
            case ScreenEdgeType.Right:
                this.Transform.Wid = width;
                this.Transform.Ht = height;
                this.Transform.X = width;
                this.Transform.Y = 0;
                break;
        }

        this.Transform.recalculateCentre();
        MyBody.addRectCollider();
    }

    public void onCollisionEnter(PhysicsBody x)
    {
        if (x.Parent.checkTag("Bullet") == false)
        {
            MyBody.DebugColor = Color.Red;
        }
    }

    public void handleInput(InputEvent ignored, string alsoIgnored)
    {
    }

    public void onCollisionExit(PhysicsBody x)
    {
        MyBody.DebugColor = Color.Green;
    }

    public void onCollisionStay(PhysicsBody x)
    {
        MyBody.DebugColor = Color.Blue;
    }

    public override void update()
    {
        Bootstrap.getDisplay().addToDraw(this);
    }
}