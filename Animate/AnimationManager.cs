using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Alakoz.Animate
{
	public class AnimationManager
	{
		// ========================================================== VARIABLES ==========================================================
		public const float FPS24 = 0.024f; // milliseconds
		private float _timer; // Stopwatch for the animation
		private int _frame = 0;
		private string _frameMSG = "";
		private bool _paused = false; // to Pause the animation
		private bool displayFrames { get; set; }
		public bool isDone { get; set; } // Checks if the animation is finished
		public Vector2 Position { get; set; } // Position of the animation
		private Animation _animation {get; set;} // Corresponding animation
		private Stack<Animation> _animationStack {get; set;} // Stack of animations to play in sequence
		private Animation _tempAnimation {get; set;} // Just to store the looping animation when popping from the stac
		public SpriteFont frameFont { get; set; } // Display message font

		// ========================================================== CONSTRUCTOR ==========================================================
		public AnimationManager(Animation newAnimation, bool display = false)
		{
			_animation = newAnimation;
			displayFrames = display;
			_animationStack = new Stack<Animation>();
		}
		
		// ========================================================== STACK FUNCTIONS ==========================================================
		public int Frame() {return _frame;} // Return the current frame of the animation
		public void Reset() // Reset the current animation to start from the beginning
		{
			_timer = 0;
			_frame = 0;
			isDone = false;
		}
		public void Clear() {_animationStack.Clear();} // Clear all animations
		public void Add(Animation newItem){ _animationStack.Push(newItem);} // Add an animation
		public void Pause(){_paused = true;} // Pause the animation on the current frame
		public void Resume(){_paused = false;} // Reusme the animation
		public Animation Remove(){ return _animationStack.Pop();}
		
		/// Play the animation at the top of the stack
		public void Play()
		{
			Animation newAnimation = _animationStack.Pop();
			if (_animation == newAnimation) return;

			_animation = newAnimation;
			_tempAnimation = newAnimation;
			Reset(); // Start the animation from the beginning
		}
		
		/// Play the next animation in the stack
		private void Next()
		{
			if (_animationStack.Count != 0) Play(); // Play the next animation
			else { if (_animation.isLooping) Reset();} // There is only one looping animation which is at the bottom of the stack
		}

		// ========================================================== UPDATING ==========================================================

		public virtual void Update(GameTime gameTime)
		{
			// Hold current frame if pauesd
			if (_paused ) return; 

			// How much time has passed ( in seconds ) since the last update call
			_timer += (float)gameTime.ElapsedGameTime.TotalSeconds; 
			
			_frameMSG = "Animation Frame: " + _frame.ToString(); // Display message

            if (_timer >= _animation.frameSpeed && isDone == false)
			{
				// offset the timer 
				_timer = _animation.frameSpeed - _timer;

				// Update the frame counts
				_frame++;
				
				// This means that the animation is done
				if (_frame >= _animation.totalFrames) Next();
			}
		}
		// ========================================================== DRAWING ==========================================================

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 scale, SpriteEffects spriteEffects)
		{
			spriteBatch.Draw(
				_animation.Sprite,
				position,
                new Rectangle(_frame * _animation.frameWidth,
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

