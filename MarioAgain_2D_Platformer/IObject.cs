/* IObject.cs
 * Author: Mark DiVelbiss
 * IObject is the interface for all objects in the game.
 * Worlds contain quadtrees full of all objects that exist in the world, updating and drawing them every frame.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarioAgain.Objects
{
    public interface IObject
    {
        ObjectType Type { get; set; }
        ObjectMode Mode { get; set; }
        Rectangle Mask { get; }

        void Execute(string com);
        void Poke(CollideType type, IObject source);
        void Kill();

        void Update();
        void Draw(SpriteBatch spriteBatch);
    }

    public enum ObjectType { None, Player, DeadPlayer, Enemy, DeadEnemy, Solid };
    public enum CollideType { Death, PlayerAttack, EnemyAttack, Hazard, Bounce, Explode, RedMushroom, GreenMushroom, BlueMushroom, FireFlower, Star };
    public enum ObjectMode { Static, Dynamic, Null };
}
