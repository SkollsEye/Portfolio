/* Star.cs
 * Author: Mark DiVelbiss
 * This class defines the Star IObject, which is the Start powerup from the original Super Mario Bros.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioAgain.Assets;
using MarioAgain.Utilities;
using MarioAgain.Worlds;
using MarioAgain.Objects.StarStates;

namespace MarioAgain.Objects
{
    class Star : IObject
    {
        World _world;
        IStarState _state;
        Texture2D _asset;
        SpriteData _spriteData;
        CollideType _ctype;

        Vector2 position;
        float xVelocity, yVelocity;

        const float xVelocityMax = 2, yVelocityMax = 3, riseVelocity = -1, jumpVelocity = -5;

        public Star(World world, Texture2D asset, SpriteData spriteData, CollideType ctype, int x, int y)
        {
            Type = ObjectType.None;
            Mode = ObjectMode.Dynamic;

            _world = world;
            _asset = asset;
            _spriteData = spriteData;
            _ctype = ctype;

            position = new Vector2(x, y);
            xVelocity = 0; yVelocity = 0;
            _state = new StarSpawnState(this);
            spriteData.Play("Normal");
        }

        //////////////////////////////////////////// IObject Methods

        public ObjectType Type { get; set; }
        public ObjectMode Mode { get; set; }
        public Rectangle Mask { get { return new Rectangle((int)position.X, (int)position.Y, _spriteData.GetSourceRect().Width, _spriteData.GetSourceRect().Height); } }

        public void Execute(string com) { }
        public void Poke(CollideType type, IObject source)
        {
            switch (type)
            {
                case CollideType.Bounce:
                    StartJump();
                    break;
            }
        }
        public void Kill() { Mode = ObjectMode.Null; }

        public void Update()
        {
            yVelocity = _world.physics.Gravity(yVelocity, yVelocityMax);
            xVelocity = _world.physics.Friction(xVelocity);

            _state.Update();
            _spriteData.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_asset, Mask, _spriteData.GetSourceRect(), Color.White, 0, Vector2.Zero, SpriteEffects.None, Properties.LayerMGBack);
        }

        //////////////////////////////////////////// Star Methods

        public void ChangeState(IStarState newState) { _state = newState; }

        public void MoveRight() { xVelocity = xVelocityMax; }
        public void MoveLeft() { xVelocity = -xVelocityMax; }
        public void StartJump() { yVelocity = jumpVelocity; }
        public void RiseUp() { yVelocity = riseVelocity; }

        public bool CheckSolidCollision(Rectangle mask) { return _world.QueryCollision(Rectangle.Empty, mask, ObjectType.Solid) != null; }
        public void FindPlayerCollision()
        {
            IObject obj = _world.QueryCollision(Rectangle.Empty, Mask, ObjectType.Player);
            if (obj != null)
            {
                obj.Poke(_ctype, this);
                Kill();
            }
        }

        public void FreeMove()
        {
            position.X += (float)_world.physics.RoundAwayFromZero(xVelocity);
            position.Y += (float)_world.physics.RoundAwayFromZero(yVelocity);
        }

        public void SolidMove()
        {
            xVelocity = _world.physics.TypeMoveX(Mask, ObjectType.Solid, xVelocity);
            position.X += (float)_world.physics.RoundAwayFromZero(xVelocity);
            yVelocity = _world.physics.TypeMoveY(Mask, ObjectType.Solid, yVelocity);
            position.Y += (float)_world.physics.RoundAwayFromZero(yVelocity);
            position = _world.physics.EscapeType(Mask, ObjectType.Solid);
        }
    }
}
