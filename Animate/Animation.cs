using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alakoz.Animate
{
	public class Animation
	{
		public int currentFrame { get; set; } // The current frame

		public int totalFrames { get; set; } // Total number of frames in the animation

		public int frameHeight { get { return Sprite.Height; } } // Height of the sprite sheet

		public int frameWidth { get { return Sprite.Width / totalFrames;  } } // Width of a single frame

		public float frameSpeed { get; set; } // How long each frame should be displayed

		public bool looping { get; set; } // variable for looping animation

		public Texture2D Sprite { get; set; } // Sprite sheet 

		public Animation(Texture2D newSprite, int numFrames, bool loop = true)
		{
			Sprite = newSprite;
			totalFrames = numFrames;
			looping =  loop;
		}
	}
}

