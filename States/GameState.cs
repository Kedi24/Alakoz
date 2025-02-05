using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.GameObjects;
using Alakoz.Collision;

using TiledCS;

namespace Alakoz.States;

public abstract class GameState
{
    public const string testState = "TestState";
    public string type {get; set;}
    public List<GameObject> allGameObjects; // All the species that need to be updated and drawn
    public abstract void Initialize();
    public abstract void LoadContent(); 
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

}