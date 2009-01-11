// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

namespace Bnoerj { namespace Winshoked {

	using namespace System;
	using namespace Microsoft::Xna::Framework;

	/// <summary>
	/// The WinshokedComponent provides properties and methods to disable the
	/// Windows and accessibility shortcut keys.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public ref class WinshokedComponent : public GameComponent
	{
		bool disableAccessibilityShortcutKeys;

	internal:

		static WinshokedComponent^ Instance = nullptr;

	public:

		/// <summary>
		/// Initializes a new instance of the WinshokedComponent class.
		/// </summary>
		/// <param name="game">Game that the game component should be attached to.</param>
		WinshokedComponent(Microsoft::Xna::Framework::Game^ game)
			: GameComponent(game)
			, disableAccessibilityShortcutKeys(true)
		{
			if (WinshokedComponent::Instance != nullptr)
			{
				throw gcnew InvalidOperationException("Todo");
			}

			WinshokedComponent::Instance = this;
		}

		~WinshokedComponent()
		{
			this->!WinshokedComponent();

			GC::SuppressFinalize(this);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the accessibility shortcut
		/// keys are disabled.
		/// </summary>
		property bool DisableAccessibilityShortcutKeys
		{
			bool get();
			void set(bool value);
		}

		/// <summary>
		/// Called when the GameComponent needs to be initialized.
		/// </summary>
		virtual void Initialize() override;

	protected:

		!WinshokedComponent();

		virtual void OnEnabledChanged(Object^ sender, EventArgs^ args) override;

		void Game_Activated(Object^ sender, EventArgs^ e);
		void Game_Deactivated(Object^ sender, EventArgs^ e);
	};
}}
