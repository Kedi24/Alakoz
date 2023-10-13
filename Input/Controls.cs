using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Alakoz.Animate;
using Alakoz.Input;

namespace Alakoz.Input
{
	public class Controls
	{
		public Keys Left { get; set; }
		public Keys Right { get; set; }
		public Keys Up { get; set; }
		public Keys Down { get; set; }
		public Keys Jump { get; set; }
		public Keys Crouch { get; set; }
		public Keys Dash { get; set; }
		public Keys Attack { get; set; }
		public Keys Skill { get; set; }

		public Controls()
		{
			reset();
			// We can pass in strings that correspond to default values
			// ex. DEFAULT, ADVANCED, etc
			
		}

		/// Checks if a Key is down ( or up )
		public bool isDown(Keys currentKey)
		{
			return Keyboard.GetState().IsKeyDown(currentKey);
		}

        public bool isUp(Keys currentKey)
        {
			return Keyboard.GetState().IsKeyUp(currentKey);
        }


		/// resets the current keyboard configuration to default settings
				public void reset()
		{
			// DEFAULT CONTROLS
			Left = Keys.A;
            Right = Keys.D;
            Up = Keys.E;
            Down = Keys.X;
            Jump = Keys.W;
            Crouch = Keys.S;
            Dash = Keys.F;
            Attack = Keys.R;
            Skill = Keys.T;
        }

		public bool isAllUp()
		{
			if ( Keyboard.GetState().IsKeyDown(Left)
			|| Keyboard.GetState().IsKeyDown(Right)
			|| Keyboard.GetState().IsKeyDown(Up)
			|| Keyboard.GetState().IsKeyDown(Down)			
			|| Keyboard.GetState().IsKeyDown(Jump)
			|| Keyboard.GetState().IsKeyDown(Crouch)
			|| Keyboard.GetState().IsKeyDown(Dash)
			|| Keyboard.GetState().IsKeyDown(Attack)
			|| Keyboard.GetState().IsKeyDown(Skill)
			|| Keyboard.GetState().IsKeyDown(Left) )	
			{
				return false;
			}	
			else 
			{
				return true;
			}	
		}
		public void altControls()
		{
			Left = Keys.J;
            Right = Keys.L;
            Up = Keys.U;
            Down = Keys.M;
            Jump = Keys.I;
            Crouch = Keys.K;
            Dash = Keys.H;
            Attack = Keys.Y;
            Skill = Keys.G;
		}
    }
}

