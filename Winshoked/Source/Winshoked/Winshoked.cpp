// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"
#include "Winshoked.h"

namespace Bnoerj { namespace Winshoked { namespace Native {

	HHOOK KeyboardHookHandle = NULL;
	STICKYKEYS StartupStickyKeys = {sizeof(STICKYKEYS), 0};
	TOGGLEKEYS StartupToggleKeys = {sizeof(TOGGLEKEYS), 0};
	FILTERKEYS StartupFilterKeys = {sizeof(FILTERKEYS), 0};

	LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
	{
		if (nCode < 0 || nCode != HC_ACTION)
		{
			// do not process message
			return CallNextHookEx(KeyboardHookHandle, nCode, wParam, lParam);
		}

		KBDLLHOOKSTRUCT* p = (KBDLLHOOKSTRUCT*)lParam;
		if (wParam == WM_KEYDOWN || wParam == WM_KEYUP)
		{
			bool isWinKey = p->vkCode == VK_LWIN || p->vkCode == VK_RWIN;
			if (WinshokedComponent::Instance->Enabled == true &&
				WinshokedComponent::Instance->Game->IsActive == true &&
				isWinKey == true)
			{
				return 1;
			}
		}

		return CallNextHookEx(KeyboardHookHandle, nCode, wParam, lParam);
	}

	void DisableAccessibilityShortcutKeys(bool disable)
	{
		// REVIEW: Only disable keys when in fullscreen?
		if (disable == false)
		{
			// Restore StickyKeys/etc to original state and enable Windows key      

			SystemParametersInfo(SPI_SETSTICKYKEYS, sizeof(STICKYKEYS), &StartupStickyKeys, 0);
			SystemParametersInfo(SPI_SETTOGGLEKEYS, sizeof(TOGGLEKEYS), &StartupToggleKeys, 0);
			SystemParametersInfo(SPI_SETFILTERKEYS, sizeof(FILTERKEYS), &StartupFilterKeys, 0);
		}
		else
		{
			// Disable StickyKeys/etc shortcuts but if the accessibility feature is on, 
			// then leave the settings alone as its probably being usefully used

			STICKYKEYS skOff = StartupStickyKeys;
			if ((skOff.dwFlags & SKF_STICKYKEYSON) == 0)
			{
				// Disable the hotkey and the confirmation
				skOff.dwFlags &= ~SKF_HOTKEYACTIVE;
				skOff.dwFlags &= ~SKF_CONFIRMHOTKEY;

				SystemParametersInfo(SPI_SETSTICKYKEYS, sizeof(STICKYKEYS), &skOff, 0);
			}

			TOGGLEKEYS tkOff = StartupToggleKeys;
			if ((tkOff.dwFlags & TKF_TOGGLEKEYSON) == 0)
			{
				// Disable the hotkey and the confirmation
				tkOff.dwFlags &= ~TKF_HOTKEYACTIVE;
				tkOff.dwFlags &= ~TKF_CONFIRMHOTKEY;

				SystemParametersInfo(SPI_SETTOGGLEKEYS, sizeof(TOGGLEKEYS), &tkOff, 0);
			}

			FILTERKEYS fkOff = StartupFilterKeys;
			if ((fkOff.dwFlags & FKF_FILTERKEYSON) == 0)
			{
				// Disable the hotkey and the confirmation
				fkOff.dwFlags &= ~FKF_HOTKEYACTIVE;
				fkOff.dwFlags &= ~FKF_CONFIRMHOTKEY;

				SystemParametersInfo(SPI_SETFILTERKEYS, sizeof(FILTERKEYS), &fkOff, 0);
			}
		}
	}

}}}

using namespace Bnoerj::Winshoked;

WinshokedComponent::!WinshokedComponent()
{
	if (Native::KeyboardHookHandle != NULL)
	{
		::UnhookWindowsHookEx(Native::KeyboardHookHandle);

		Native::KeyboardHookHandle = NULL;
	}

	Native::DisableAccessibilityShortcutKeys(false);
}

bool WinshokedComponent::DisableAccessibilityShortcutKeys::get()
{
	return disableAccessibilityShortcutKeys;
}

void WinshokedComponent::DisableAccessibilityShortcutKeys::set(bool value)
{
	if (disableAccessibilityShortcutKeys != value)
	{
		disableAccessibilityShortcutKeys = value;

		// Only disable accessibility shortcut keys when the component is enabled
		// and the game is active
		if (disableAccessibilityShortcutKeys == true && Enabled == true && Game->IsActive == true)
		{
			Native::DisableAccessibilityShortcutKeys(true);
		}
		else if (disableAccessibilityShortcutKeys == false)
		{
			Native::DisableAccessibilityShortcutKeys(false);
		}
	}
}

void WinshokedComponent::Initialize()
{
	__super::Initialize();

	if (Native::KeyboardHookHandle != NULL)
	{
		throw gcnew InvalidOperationException("Todo");
	}

	Native::KeyboardHookHandle = ::SetWindowsHookEx(
		WH_KEYBOARD_LL,
		Native::LowLevelKeyboardProc,
		::GetModuleHandle(NULL),
		0);

	// Save the current sticky/toggle/filter key settings so they can be restored them later
	::SystemParametersInfo(SPI_GETSTICKYKEYS, sizeof(STICKYKEYS), &Native::StartupStickyKeys, 0);
	::SystemParametersInfo(SPI_GETTOGGLEKEYS, sizeof(TOGGLEKEYS), &Native::StartupToggleKeys, 0);
	::SystemParametersInfo(SPI_GETFILTERKEYS, sizeof(FILTERKEYS), &Native::StartupFilterKeys, 0);

	Game->Activated += gcnew EventHandler(this, &WinshokedComponent::Game_Activated);
	Game->Deactivated += gcnew EventHandler(this, &WinshokedComponent::Game_Deactivated);
}

void WinshokedComponent::OnEnabledChanged(Object^ sender, EventArgs^ args)
{
	__super::OnEnabledChanged(sender, args);

	Native::DisableAccessibilityShortcutKeys(Enabled == true ? disableAccessibilityShortcutKeys : false);
}

void WinshokedComponent::Game_Activated(Object^ /*sender*/, EventArgs^ /*e*/)
{
	Native::DisableAccessibilityShortcutKeys(Enabled == true ? disableAccessibilityShortcutKeys : false);
}

void WinshokedComponent::Game_Deactivated(Object^ /*sender*/, EventArgs^ /*e*/)
{
	Native::DisableAccessibilityShortcutKeys(false);
}
