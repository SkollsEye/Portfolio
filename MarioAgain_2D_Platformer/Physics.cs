/* Physics.cs
 * Author: Mark DiVelbiss
 * The Physics class is a public utility class intended to universalize common methods for movement and collision between objects.
 * Specifically, this class is intended to handle acceleration, friction, gravity, collision detection and collision correction.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MarioAgain.Objects;
using MarioAgain.Utilities;

namespace MarioAgain.Worlds
{
    class Physics
    {
        World _world;
        const float ACCELERATION = 0.5f;
        const float FRICTION = 0.25f;
        const float GRAVITY = 0.2f;

        public Physics(World world) { _world = world; }

        public float TypeMoveX(Rectangle objMask, ObjectType type, float xVelocity)
        {
            Rectangle newMask = objMask;
            newMask.X = objMask.X + (int)RoundAwayFromZero(xVelocity);
            if (_world.QueryCollision(objMask, newMask, type) == null) return xVelocity;

            xVelocity = (float)RoundAwayFromZero(xVelocity);
            if (xVelocity > 0)
            {
                while (xVelocity > 0)
                {
                    newMask.X = objMask.X + (int)xVelocity;
                    if (_world.QueryCollision(objMask, newMask, type) == null) return xVelocity;
                    xVelocity--;
                }
            }
            else if (xVelocity < 0)
            {
                while (xVelocity < 0)
                {
                    newMask.X = objMask.X + (int)xVelocity;
                    if (_world.QueryCollision(objMask, newMask, type) == null) return xVelocity;
                    xVelocity++;
                }
            }
            return 0;
        }

        public float TypeMoveY(Rectangle objMask, ObjectType type, float yVelocity)
        {
            Rectangle newMask = objMask;
            newMask.Y = objMask.Y + (int)RoundAwayFromZero(yVelocity);
            if (_world.QueryCollision(objMask, newMask, type) == null) return yVelocity;
            
            yVelocity = (float)RoundAwayFromZero(yVelocity);
            if (yVelocity > 0)
            {
                while (yVelocity > 0)
                {
                    newMask.Y = objMask.Y + (int)yVelocity;
                    if (_world.QueryCollision(objMask, newMask, type) == null) return yVelocity;
                    yVelocity--;
                }
            }
            else if (yVelocity < 0)
            {
                while (yVelocity < 0)
                {
                    newMask.Y = objMask.Y + (int)yVelocity;
                    if (_world.QueryCollision(objMask, newMask, type) == null) return yVelocity;
                    yVelocity++;
                }
            }
            return 0;
        }

        public Vector2 EscapeType(Rectangle originalMask, ObjectType type)
        {
            Rectangle newMask = originalMask;
            if (_world.QueryCollision(originalMask, newMask, type) == null) return new Vector2(newMask.X, newMask.Y);

            for (int tolerance = 1; tolerance < Properties.worldLimits.Width; tolerance++)
            {
                newMask.Y -= tolerance;
                if (_world.QueryCollision(originalMask, newMask, type) == null) break;
                newMask.Y += 2 * tolerance;
                if (_world.QueryCollision(originalMask, newMask, type) == null) break;
                newMask.Y -= tolerance;
                newMask.X -= tolerance;
                if (_world.QueryCollision(originalMask, newMask, type) == null) break;
                newMask.X += 2 * tolerance;
                if (_world.QueryCollision(originalMask, newMask, type) == null) break;
                newMask.X -= tolerance;
            }
            return new Vector2(newMask.X, newMask.Y);
        }

        public float Accel(float velocity, float maxVelocity, bool positive = true)
        {
            if (positive)
            {
                velocity = velocity + ACCELERATION;
                if (velocity > maxVelocity) velocity = maxVelocity;
            }
            else
            {
                velocity = velocity - ACCELERATION;
                if (velocity < -maxVelocity) velocity = -maxVelocity;
            }
            return velocity;
        }

        public float Friction(float velocity)
        {
            if (velocity > 0)
            {
                velocity = velocity - FRICTION;
                if (velocity < 0) velocity = 0;
            }
            else if (velocity < 0)
            {
                velocity = velocity + FRICTION;
                if (velocity > 0) velocity = 0;
            }
            return velocity;
        }

        public float Gravity(float yVelocity, float maxVelocity)
        {
            yVelocity += GRAVITY;
            if (yVelocity > maxVelocity) yVelocity = maxVelocity;
            return yVelocity;
        }

        public bool Grounded(Rectangle mask)
        {
            Rectangle groundTestMask = mask;
            groundTestMask.Y++;
            return _world.QueryCollision(mask, mask, ObjectType.Solid) == null && _world.QueryCollision(mask, groundTestMask, ObjectType.Solid) != null;
        }

        public bool Ceiling(Rectangle mask)
        {
            Rectangle groundTestMask = mask;
            groundTestMask.Y--;
            return _world.QueryCollision(mask, mask, ObjectType.Solid) == null && _world.QueryCollision(mask, groundTestMask, ObjectType.Solid) != null;
        }

        public float RoundAwayFromZero(float number)
        {
            if (number >= 0) return (float)Math.Ceiling(number);
            else return (float)Math.Floor(number);
        }
    }
}
