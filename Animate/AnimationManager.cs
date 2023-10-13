using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;
using Alakoz.LivingBeings;
using Alakoz.Collision;

namespace Alakoz.Animate
{
	public class AnimationManager
	{
		public const float FPS24 = 0.024f; // milliseconds

		private int _frame = 0; // Current Frame

		private string _frameMSG = ""; // Display message for the frames

		private bool displayFrames { get; set; } // Toggle to show the frame counter

		private Animation _animation; // Corresponding animation

		private float _timer; // Stopwatch for the animation

		public bool isDone { get; set; } // Checks if the animation is finished

		public Vector2 Position { get; set; } // Position of the animation

		public SpriteFont frameFont { get; set; } // Font for the display message

		public AnimationManager(Animation newAnimation)
		{
			_animation = newAnimation;
			displayFrames = false;
		}
		public AnimationManager(Animation newAnimation, bool display) :this(newAnimation) // Calls the above constructure and sets the display value
		{
			displayFrames = display;
		}
		
		/// Plays the animation
		public void Play(Animation animation)
		{
			if (_animation == animation) return;

			_animation = animation;
			_animation.currentFrame = 0;
			_timer = 0;
			_frame = 0;
			isDone = false;

		}
		/// Stops the animation. Pausing it in place ( i think :| ) 
		public void Stop()
		{
			_timer = 0;
			_frame = 0;
			if (_animation.looping) _animation.currentFrame = 0;
            
			if (!_animation.looping) isDone = true;
        }

		/// Resets the animation to play from the beginning
		public void Reset(Animation tempAnimation)
		{
			_timer = 0;
			tempAnimation.currentFrame = 0;
			isDone = false;
		}

		// ----------------------------------------------------- MONOGAME FRAMEWORK FUNCTIONS -----------------------------------------------------

		public virtual void Update(GameTime gameTime)
		{
			_timer += (float)gameTime.ElapsedGameTime.TotalSeconds; // How much time has passed ( in seconds ) since the last update call

			_frameMSG = "Frame: " + _frame.ToString();

            if (_timer >= FPS24 && isDone == false)
			{
				_timer = FPS24 - _timer;

				_animation.currentFrame++;
				_frame++;
				
				if (_animation.currentFrame >= _animation.totalFrames)
				{
					this.Stop();
				}

			}
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 scale, SpriteEffects spriteEffects)
		{
			spriteBatch.Draw(
				_animation.Sprite,
				position,
                new Rectangle(_animation.currentFrame * _animation.frameWidth,
					0,
					_animation.frameWidth,
					_animation.frameHeight),
				Color.White,
				0f,
				Vector2.Zero,
				scale,
				spriteEffects,
				0f) ;
            // if (displayFrames) spriteBatch.DrawString(frameFont, _frameMSG, new Vector2(0f, 400), Color.Gold);
        }
	}
}

