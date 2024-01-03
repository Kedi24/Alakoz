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

public abstract class GameStates 
{
    public const string testState = "TestState";
    public string type {get; set;}
    public abstract void Initialize();
    public abstract void LoadContent(); 
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch,GameTime gameTime);

}