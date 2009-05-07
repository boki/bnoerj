namespace Bnoerj.ColorAnalyzer
{
	using System;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	/// <summary>
	/// Defines methods and properties for post processing color effects.
	/// </summary>
	class ColorEffect : IDisposable
	{
		/// <summary>
		/// The XNA Framework effect.
		/// </summary>
		protected Effect effect;

		/// <summary>
		/// The viewpoert size paramter.
		/// </summary>
		EffectParameter paramViewportSize;

		/// <summary>
		/// The texture parameter.
		/// </summary>
		EffectParameter paramTexture;

		/// <summary>
		/// Initializes a new instance of the ColorEffect class.
		/// </summary>
		/// <param name="effect">The XNA Framework effect.</param>
		protected ColorEffect(Effect effect)
		{
			this.effect = effect;
			this.paramViewportSize = effect.Parameters["ViewportSize"];
			this.paramTexture = effect.Parameters["Texture"];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="colorEffect"></param>
		/// <returns></returns>
		public static implicit operator Effect(ColorEffect colorEffect)
		{
			return colorEffect.effect;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		public static implicit operator ColorEffect(Effect effect)
		{
			return new ColorEffect(effect);
		}

		/// <summary>
		/// Sets the viewport size.
		/// </summary>
		public Vector2 ViewportSize
		{
			set { paramViewportSize.SetValue(value); }
		}

		/// <summary>
		/// Sets the texture to process.
		/// </summary>
		public Texture2D Texture
		{
			set { paramTexture.SetValue(value); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public EffectPassCollection Begin()
		{
			effect.Begin();
			return effect.CurrentTechnique.Passes;
		}

		/// <summary>
		/// 
		/// </summary>
		public void End()
		{
			effect.End();
		}

		/// <summary>
		/// 
		/// </summary>
		public void CommitChanges()
		{
			effect.CommitChanges();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			effect.Dispose();
		}
	}
}
