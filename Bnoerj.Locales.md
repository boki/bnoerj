**Bnoerj.Locales** [release:Current release](3980)

This library provides support for localized keyboard layouts, a text input service and a supporting text renderer.

Locales, their keyboard layout and the font are defined at compile time through a simple XML file:
{{<Locale Name="en-US">
    <Font Name="Verdana" />
</Locale>}}
The locales name corresponds to the accepted values for the CultureInfo constuctor.
The fonts name is the friendly name of the font to import.

The Locale class holds the keyboard layout and the supporting font texture.

To use the Locale class in a game, simply instantiate a LocaleService. The LocaleService will automatically load the definiton corresponding to the users current locale and fallback to en-US, if the users locale is not supported.

The TextInputService provides a simple interface for text input, applying application defined keyboard delay and repeat speeds.

The TextRenderer class implements text drawing, mimicing the TextRenderer of .NET 2.0.