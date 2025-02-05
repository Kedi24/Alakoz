using System.Collections.Generic;
using Alakoz.GameInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alakoz.Animate
{
	public class AnimationManager
	{
		public int _frame = 1;
		public bool _paused; // to Pause the animation
		private int _delayDefault = 2;
        private int _delay = 2; // to match FPS with animation rate. FPS = 60, Animation = 20 or 30fps
		public Animation _animation {get; set;} // Corresponding animation
		public Stack<Animation> _animationStack {get; set;} // Stack of animations to play in sequence
		public AnimationManager(Animation newAnimation){
			_animation = newAnimation;  
			_animationStack = new Stack<Animation>();
		}
		public TState getName(){return _animation.Name;}
		public int Frame() {return _frame;} // Return the current frame of the animation
		public void Reset() {_frame = 1;}// Reset the current animation to start from the beginning
		public void Clear() {_animationStack.Clear();} // Clear all animations
		public void Add(Animation newItem){ _animationStack.Push(newItem);} // Add an animation
		public void Pause(){_paused = true;} // Pause the animation on the current frame
		public void Resume(){_paused = false;} // Reusme the animation
		 
		public void Play(){
			Animation newAnimation = _animationStack.Pop();
			if (_animation == newAnimation) return;

			_animation = newAnimation;
			Reset(); // Start the animation from the beginning    
		}
		
		public void Play(Animation newAnimation, Animation[] after = null){
			Clear();
			_animation = newAnimation;
			if (after != null) foreach(var animation in after) Add(animation);
			Reset(); 
		}

		public void Next(){
			if (_animationStack.Count != 0) Play(); // Play the next animation
			else { if (_animation.isLooping) Reset();} // There is only one looping animation which is at the bottom of the stack
		}

		public virtual void Update(){
			if (_paused) return;; // Hold current frame if pauesd
			if (_delay > 1) { _delay--; return;};

			_frame++; // Update frame counter
			 _delay = _delayDefault; // Reset frame delay
			if (_frame >= _animation.Frames) Next(); // To play the next animation
		}

		public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, Vector2 scale, SpriteEffects spriteEffects){
			spriteBatch.Draw(
				_animation.Sprite,
				position,
                new Rectangle(
					_animation.Width * ((_frame - 1) % _animation.Columns), // Remainder
					_animation.Height * ((_frame - 1 - ((_frame- 1) % _animation.Columns)) / _animation.Columns) // (Dividend - Remainder) / Divisosr = Current Column
					,
					_animation.Width,
					_animation.Height),
				Color.White,
				0f,
				Vector2.Zero,
				scale,
				spriteEffects,
				0f) ;
        }
	}
}

