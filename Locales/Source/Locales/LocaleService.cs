// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Bnoerj.Locales.Text;

namespace Bnoerj.Locales
{
	/// <summary>
	/// The LocaleService 
	/// </summary>
	public class LocaleService : GameComponent
	{
		ContentManager content;
		String contentRootPath;
		Locale locale;
		// FIXME: Include TextInputService?

		/// <summary>
		/// Initializes a new instance of LocaleService.
		/// </summary>
		/// <remarks>
		/// The new instance is added to the games services and components
		/// collections.
		/// </remarks>
		/// <param name="game">Game the LocaleService should be attached to.</param>
		/// <param name="content">The content manager the LocaleService should use to load locales.</param>
		public LocaleService(Game game, ContentManager content)
			: this(game, content, "Content")
		{
		}

		/// <summary>
		/// Initializes a new instance of LocaleService.
		/// </summary>
		/// <remarks>
		/// The new instance is added to the games services and components
		/// collections.
		/// </remarks>
		/// <param name="game">Game the LocaleService should be attached to.</param>
		/// <param name="content">The content manager the LocaleService should use to load locales.</param>
		/// <param name="contentRootPath">The content root path the LocaleService should use to search for locale definitions.</param>
		public LocaleService(Game game, ContentManager content, String contentRootPath)
			: base(game)
		{
			this.content = content;
			this.contentRootPath = contentRootPath;

			game.Services.AddService(typeof(LocaleService), this);
			game.Components.Add(this);
		}

		/// <summary>
		/// Gets or sets the content root path the LocaleService should use
		/// to search for locale definitions.
		/// </summary>
		/// <example>
		/// Content/Locales
		/// becomes Content/Locales/en-EN/definition.locale
		/// </example>
		public String ContentRootPath
		{
			get { return contentRootPath; }
			set { contentRootPath = value; }
		}

		/// <summary>
		/// Gets the users current Locale.
		/// </summary>
		public Locale CurrentLocale
		{
			get { return locale; }
		}

		public override void Initialize()
		{
			base.Initialize();

			// FIXME: move into LoadGraphicsContent() because of the font texture
			// Use the users locale as default
			LoadLocale(Thread.CurrentThread.CurrentCulture.Name);
		}

		public void LoadLocale(String localeName)
		{
			if (locale != null && locale.IsDisposed == false)
			{
				// FIXME: disposing the font textures will break rendering when the locale is used again
				//locale.Dispose();
			}

			String localeContentPath = String.Format("{0}/{1}/definition", contentRootPath, localeName);
			try
			{
				locale = content.Load<Locale>(localeContentPath);
			}
			catch (ContentLoadException)
			{
				// Fallback to en-US if the specified locale is not available
				// Load will throw an exception if the definition for en-US
				// is not available
				localeContentPath = String.Format("{0}/en-US/definition", contentRootPath, localeName);
				locale = content.Load<Locale>(localeContentPath);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

	}
}
