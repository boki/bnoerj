namespace Bnoerj.ColorAnalyzer
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	class ColorClippingEffect : ColorEffect
	{
		EffectParameter paramMinColor;
		EffectParameter paramMaxColor;
		EffectParameter paramBlackPoint;
		EffectParameter paramWhitePoint;

		/// <summary>
		/// Initializes a new instance of the ColorClippingEffect class.
		/// </summary>
		public ColorClippingEffect(Effect effect)
			: base(effect)
		{
			paramMinColor = effect.Parameters["MinColor"];
			paramMaxColor = effect.Parameters["MaxColor"];
			paramWhitePoint = effect.Parameters["WhitePoint"];
			paramBlackPoint = effect.Parameters["BlackPoint"];
		}

		public Vector4 MinColor
		{
			set { paramMinColor.SetValue(value); }
		}

		public Vector4 MaxColor
		{
			set { paramMaxColor.SetValue(value); }
		}

		public float BlackPoint
		{
			set { paramBlackPoint.SetValue(value); }
		}

		public float WhitePoint
		{
			set { paramWhitePoint.SetValue(value); }
		}
	}
}
