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
using Alakoz.GameInfo;

namespace Alakoz.Animate
{
	public class AnimationManager
	{
		// ========================================================== VARIABLES ==========================================================
		public const float FPS24 = 0.024f; // milliseconds
		private float _timer; // Stopwatch for the animation
		private int _frame = 0;
		private string _frameMSG = "";
		private bool _paused; // to Pause the animation
		private bool displayFrames { get; set; }
		public bool isDone { get; set; } // Checks if the animation is finished
		public Vector2 Position { get; set; } // Position of the animation
		private Animation _animation {get; set;} // Corresponding animation
		public Stack<Animation> animationStack {get; set;} // Stack of animations to play in sequence
		private Animation _tempAnimation {get; set;} // Just to store the looping animation when popping from the stac
		public SpriteFont frameFont { get; set; } // Display message font

		// ========================================================== CONSTRUCTOR ==========================================================
		public AnimationManager(Animation newAnimation, bool display = false)
		{
			_animation = newAnimation;
			displayFrames = display;
			animationStack = new Stack<Animation>();
		}
		
		// ========================================================== STACK FUNCTIONS ==========================================================
		public void Add(Animation newItem){ animationStack.Push(newItem);} // Add an Animation to the stack
		public void Clear() {animationStack.Clear();} // Clear the Animation stack

		/// Reset the current animation to the beginning
		public void Reset()
		{
			_timer = 0;
			_frame = 0;
			_animation.currentFrame = 0;
			isDone = false;
		}
		
		/// Play the animation at the top of the stack
		public void Play()
		{
			Animation newAnimation = animationStack.Pop();
			if (_animation == newAnimation) return;

			_animation = newAnimation;
			_tempAnimation = newAnimation;
			Reset(); // Start the animation from the beginning
		}
		
		/// Play the next animation in the stack
		private void Next()
		{
			if (animationStack.Count != 0) Play(); // Play the next animation
			else { if (_animation.isLooping) Reset();} // There is only one looping animation which is at the bottom of the stack
		}

		// ========================================================== UPDATING ==========================================================

		public virtual void Update(GameTime gameTime)
		{
			// How much time has passed ( in seconds ) since the last update call
			_timer += (float)gameTime.ElapsedGameTime.TotalSeconds; 
			
			_frameMSG = "Animation Frame: " + _frame.ToString(); // Display message

            if (_timer >= _animation.frameSpeed && isDone == false)
			{
				// offset the timer 
				_timer = _animation.frameSpeed - _timer;

				// Update the frame counts
				_animation.currentFrame++;
				_frame++;
				
				// This means that the animation is done
				if (_animation.currentFrame >= _animation.totalFrames) Next();
			}
		}


		// ========================================================== DRAWING ==========================================================

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
            // if (displayFrames) spriteBatch.DrawString(frameFont, _frameMSG, position, Color.Gold, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
		}
	}
}

