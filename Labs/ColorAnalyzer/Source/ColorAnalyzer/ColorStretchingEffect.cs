namespace Bnoerj.ColorAnalyzer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.Xna.Framework.Graphics;
	using Microsoft.Xna.Framework;

	class ColorStretchingEffect : ColorEffect
	{
		EffectParameter paramMinColor;
		EffectParameter paramMaxColor;

		/// <summary>
		/// Initializes a new instance of the ColorClippingEffect class.
		/// </summary>
		public ColorStretchingEffect(Effect effect)
			: base(effect)
		{
			paramMinColor = effect.Parameters["MinColor"];
			paramMaxColor = effect.Parameters["MaxColor"];
		}

		public Vector4 MinColor
		{
			set { paramMinColor.SetValue(value); }
		}

		public Vector4 MaxColor
		{
			set { paramMaxColor.SetValue(value); }
		}
	}
}
