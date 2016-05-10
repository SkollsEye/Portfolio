/* Quadtree.cs
 * Author: Mark DiVelbiss
 * A quadtree implementation for storing a set of IObjects in the smallest possible node on the tree.
 * The tree is self-organizing and handles collision queries and broadcasts.
 * A single quadtree is used per world.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarioAgain.Objects;
using MarioAgain.Utilities;
using MarioAgain.Assets;

namespace MarioAgain.Worlds
{
    class Quadtree
    {
        Rectangle _boundary;
        List<IObject> _objList;
        Quadtree _root, _NE, _NW, _SW, _SE;
        bool isDivided;

        public Quadtree(Quadtree root, Rectangle boundary)
        {
            _root = root;
            _boundary = boundary;
            _objList = new List<IObject>();
            isDivided = false;
        }

        private bool EmptyMask(Rectangle range) { return range.Width * range.Height <= 0; }

        private bool Subset(Rectangle range)
        {
            if (_boundary.X <= range.X &&
                _boundary.Right >= range.Right &&
                _boundary.Y <= range.Y &&
                _boundary.Bottom >= range.Bottom)
                return true;
            else return false;
        }

        private void FindSupersetFor(IObject obj)
        {
            if (Subset(obj.Mask)) Insert(obj);
            else if (_root != null) _root.FindSupersetFor(obj);
            else obj.Kill();
        }

        public bool Insert(IObject obj)
        {
            if (!Subset(obj.Mask)) return false;
            else if (EmptyMask(obj.Mask)) { _objList.Add(obj); return true; }
            else if (!isDivided)
            {
                if ((_boundary.Center.X < obj.Mask.Right && _boundary.Center.X > obj.Mask.X) ||
                    (_boundary.Center.Y < obj.Mask.Bottom && _boundary.Center.Y > obj.Mask.Y))
                { _objList.Add(obj); return true; }

                Subdivide();
            }

            if (_NE.Insert(obj)) return true;
            if (_NW.Insert(obj)) return true;
            if (_SW.Insert(obj)) return true;
            if (_SE.Insert(obj)) return true;

            _objList.Add(obj);
            return true;
        }

        public IObject QueryCollision(Rectangle identityMask, Rectangle testMask, ObjectType type)
        {
            if (!_boundary.Intersects(testMask)) return null;

            foreach (IObject obj in _objList)
                if (obj.Type == type && obj.Mask.Intersects(testMask) && !obj.Mask.Equals(identityMask))
                    return obj;

            if (!isDivided) return null;

            return _NE.QueryCollision(identityMask, testMask, type) ??
                   _NW.QueryCollision(identityMask, testMask, type) ??
                   _SW.QueryCollision(identityMask, testMask, type) ??
                   _SE.QueryCollision(identityMask, testMask, type);
        }

        public void BroadcastCollision(IObject identity, Rectangle testMask, ObjectType type, CollideType poke)
        {
            if (!_boundary.Intersects(testMask)) return;

            foreach (IObject obj in _objList)
                if (obj.Type == type && obj.Mask.Intersects(testMask) && !obj.Mask.Equals(identity.Mask))
                    obj.Poke(poke, identity);

            if (!isDivided) return;

            _NE.BroadcastCollision(identity, testMask, type, poke);
            _NW.BroadcastCollision(identity, testMask, type, poke);
            _SW.BroadcastCollision(identity, testMask, type, poke);
            _SE.BroadcastCollision(identity, testMask, type, poke);
        }

        public void BroadcastCollision(IObject identity, Rectangle testMask, CollideType poke)
        {
            if (!_boundary.Intersects(testMask)) return;

            foreach (IObject obj in _objList)
                if (obj.Mask.Intersects(testMask) && !obj.Mask.Equals(identity.Mask))
                    obj.Poke(poke, identity);

            if (!isDivided) return;

            _NE.BroadcastCollision(identity, testMask, poke);
            _NW.BroadcastCollision(identity, testMask, poke);
            _SW.BroadcastCollision(identity, testMask, poke);
            _SE.BroadcastCollision(identity, testMask, poke);
        }

        public void Subdivide()
        {
            int halfWidth = _boundary.Width / 2;
            int halfHeight = _boundary.Height / 2;

            if (halfWidth < 1) halfWidth = 1;
            if (halfHeight < 1) halfHeight = 1;

            _NE = new Quadtree(this, new Rectangle(_boundary.Left + halfWidth, _boundary.Top, halfWidth, halfHeight));
            _NW = new Quadtree(this, new Rectangle(_boundary.Left, _boundary.Top, halfWidth, halfHeight));
            _SW = new Quadtree(this, new Rectangle(_boundary.Left, _boundary.Top + halfHeight, halfWidth, halfHeight));
            _SE = new Quadtree(this, new Rectangle(_boundary.Left + halfWidth, _boundary.Top + halfHeight, halfWidth, halfHeight));

            isDivided = true;
        }

        public void UpdateTree()
        {
            MoveObjectsAsNeeded();
            UndivideAsNeeded();
        }

        public void MoveObjectsAsNeeded()
        {
            for(int i = _objList.Count - 1; i >= 0; i--)
            {
                if(_objList[i].Mode != ObjectMode.Static)
                {
                    IObject obj = _objList[i];
                    _objList.Remove(obj);
                    if (obj.Mode == ObjectMode.Dynamic) FindSupersetFor(obj);
                }
            }

            if(isDivided)
            {
                _NE.MoveObjectsAsNeeded();
                _NW.MoveObjectsAsNeeded();
                _SW.MoveObjectsAsNeeded();
                _SE.MoveObjectsAsNeeded();
            }
        }

        public void UndivideAsNeeded()
        {
            if (!isDivided) return;

            if (_NE.CanUndivide &&
                _NW.CanUndivide &&
                _SW.CanUndivide &&
                _SE.CanUndivide)
            {
                _NE._root = null; _NE = null;
                _NW._root = null; _NW = null;
                _SW._root = null; _SW = null;
                _SE._root = null; _SE = null;
                isDivided = false;
            }
            else
            {
                _NE.UndivideAsNeeded();
                _NW.UndivideAsNeeded();
                _SW.UndivideAsNeeded();
                _SE.UndivideAsNeeded();
            }
        }

        public bool CanUndivide { get { return !isDivided && _objList.Count == 0; } }

        public void UpdateObjects()
        {
            for (int i = 0; i < _objList.Count; i++)
                _objList[i].Update();

            if (isDivided)
            {
                _NE.UpdateObjects();
                _NW.UpdateObjects();
                _SW.UpdateObjects();
                _SE.UpdateObjects();
            }
        }

        public void DrawRange(SpriteBatch spriteBatch, Rectangle range)
        {
            if (!_boundary.Intersects(range)) return;

            for (int i = 0; i < _objList.Count; i++)
                _objList[i].Draw(spriteBatch);
            
          spriteBatch.Draw(Pile.redTexture, new Rectangle(_boundary.X, _boundary.Y, 1, _boundary.Height), Color.Red);
            spriteBatch.Draw(Pile.redTexture, new Rectangle(_boundary.X, _boundary.Y, _boundary.Width, 1), Color.Red);
            spriteBatch.Draw(Pile.redTexture, new Rectangle(_boundary.X + _boundary.Width - 1, _boundary.Y, 1, _boundary.Height), Color.Red);
            spriteBatch.Draw(Pile.redTexture, new Rectangle(_boundary.X, _boundary.Y + _boundary.Height - 1, _boundary.Width, 1), Color.Red);
            
            if (isDivided)
            {
                _NE.DrawRange(spriteBatch, range);
                _NW.DrawRange(spriteBatch, range);
                _SW.DrawRange(spriteBatch, range);
                _SE.DrawRange(spriteBatch, range);
            }
        }
    }
}
