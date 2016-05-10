/* World.cs
 * Author: Mark DiVelbiss
 * A single world object represents a specific scene or level within the game.
 * Menus are also worlds, and the pause menu is drawn over the game world.
 * Worlds are created from XML's with the help of WorldFactory.cs
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
    class World
    {
        Game _game;
        SoundManager _soundManager;
        bool _isPaused;
        Quadtree _objectTree;
        Scene _scene;

        public Physics physics;
        public Camera camera;

        public World(Game game)
        {
            _game = game;
            _soundManager = null;
            _isPaused = true;
            _objectTree = new Quadtree(null, Properties.worldLimits);
            _scene = null;

            Name = "";
            physics = new Physics(this);
            camera = new Camera();
        }

        public void Update()
        {
            if(_soundManager != null && !_isPaused) _soundManager.PlayLoop();
            _objectTree.UpdateTree();
            if (_scene != null) _scene.Update();
            _objectTree.UpdateObjects();
            camera.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var screenScale = Properties.GetScreenScale();
            var viewMatrix = camera.GetTransform();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, null, null, null, null, viewMatrix * Matrix.CreateScale(screenScale));
            _objectTree.DrawRange(spriteBatch, camera.Mask);
            spriteBatch.End();
        }

        public string Name { get; set; }

        public void SetSoundManager(SoundManager soundManager)
        {
            if (_soundManager != null) _soundManager.Stop();
            _soundManager = soundManager;
            _isPaused = false;
        }
        public void PauseMusic() { if (_soundManager != null) { _soundManager.Pause(); _isPaused = true; } }

        public void SetScene(Scene scene) { if (scene != null && scene.Ready) _scene = scene; }
        public void EndScene() { _scene = null; }

        public void ResetWorld() { _game.PlayWorld(GameProgress.CurrentLevel, GameProgress.PreviousTransition); }
        public void PlayWorld(string worldName, string transition) { _game.PlayWorld(worldName, transition); }
        public Progress GameProgress { get { return _game.GameProgress; } }

        public void AddObject(IObject obj) { _objectTree.Insert(obj); }

        public IObject QueryCollision(Rectangle identityMask, Rectangle testMask, ObjectType type) { return _objectTree.QueryCollision(identityMask, testMask, type); }
        public void BroadcastCollision(IObject identity, Rectangle testMask, ObjectType type, CollideType poke) { _objectTree.BroadcastCollision(identity, testMask, type, poke); }
        public void BroadcastCollision(IObject identity, Rectangle testMask, CollideType poke) { _objectTree.BroadcastCollision(identity, testMask, poke); }
    }
}
