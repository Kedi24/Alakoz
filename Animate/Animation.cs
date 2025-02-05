using Alakoz.GameInfo;
using Microsoft.Xna.Framework.Graphics;

namespace Alakoz.Animate
{
	public class Animation
	{
		public TState Name {get; set;}
		public int Frames { get; set; } // Total number of frames in the animation
		public int Rows { get; set;} // Number of Rows
		public int Columns { get; set;} // Number of columns
		public int Height { get { return Sprite.Height / Rows; } } // Height of a single frame
		public int Width { get { return Sprite.Width / Columns;  } } // Width of a single frame
		public bool isLooping { get; set; } // variable for looping animation
		public Texture2D Sprite { get; set; } // Sprite sheet 

		public Animation(TState name, Texture2D sprite, int frames = 1, bool looping = true, int rows = 1, int columns = 1){
			Sprite = sprite;
			Name = name;
			Frames = frames;
			isLooping = looping;
			Rows = rows;
			Columns = columns == 1? frames : columns; // for now
		}
    }
}

