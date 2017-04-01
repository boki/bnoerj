**Bnoerj.Winshoked** [release:Current release](21670)

This library is a GameComponent implementing [Disabling Shortcut Keys in Games](http://msdn.microsoft.com/en-us/library/bb219746(VS.85).aspx) for the XNA FX v3.0. The provided component disables and enables the Windows shortcut keys in response to the game events {{Game.Activated}} and {{Game.Deactivated}}. More fine grained control is provided through the {{GameComponent.Enabled}} and {{GameComponent.DisableAccessibilityShortcutKeys}} properties.

**How to use Bnoerj.Winshoked**

Simply add an instance of the {{Bnoerj.Winshoked.WinshokedComponent}} to your games components collection. Enabling or disabling the component through {{Bnoerj.Winshoked.WinshokedComponent.Enabled}} will disable or enable the Windows shortcut keys respectively.

**Remarks**

Some users may not be too happy when the Windows keys and/or the accessibility shortcut keys are disabled so you might want to add an option to your games settings to enable or disable the component.