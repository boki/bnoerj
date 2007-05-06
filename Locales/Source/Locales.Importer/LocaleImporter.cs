// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Importer.KeyboardLayouts;

namespace Bnoerj.Locales.Importer
{
	[ContentImporter(".locale", DisplayName = "Locale Definition - Bnoerj", DefaultProcessor = "LocaleProcessor")]
	class LocaleImporter : ContentImporter<LocaleSource>
	{
		public override LocaleSource Import(String filename, ContentImporterContext context)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			XmlNode localeNode = doc.SelectSingleNode("//Locale");
			return CreateLocaleSource(localeNode);
		}

		LocaleSource CreateLocaleSource(XmlNode localeNode)
		{
			CultureInfo cultureInfo;
			if (localeNode.Attributes["Id"] != null)
			{
				int id = int.Parse(localeNode.Attributes["Id"].Value, CultureInfo.InvariantCulture.NumberFormat);
				cultureInfo = new CultureInfo(id);
			}
			else if (localeNode.Attributes["Name"] != null)
			{
				String name = localeNode.Attributes["Name"].Value;
				cultureInfo = new CultureInfo(name);
			}
			else
			{
				throw new InvalidContentException("Missing Locale Id or Name");
			}

			String fontName;
			XmlNode fontNode = localeNode.SelectSingleNode("//Font");
			if (fontNode != null)
			{
				if (fontNode.Attributes["Name"] == null)
				{
					throw new InvalidContentException(String.Format("Missing Name attribute in Font node for locale {0}.", cultureInfo.Name));
				}
				fontName = fontNode.Attributes["Name"].Value;
			}
			else
			{
				//FIXME: Define a default font for the culture
				fontName = "Arial";
			}

			String klcFilename = null;
			XmlNode klcNode = localeNode.SelectSingleNode("//KlcFile");
			if (klcNode != null)
			{
				if (klcNode.Attributes["Name"] == null)
				{
					throw new InvalidContentException(String.Format("Missing Name attribute in Klc node for locale {0}.", cultureInfo.Name));
				}
				klcFilename = klcNode.Attributes["Name"].Value;
			}
			return new LocaleSource(cultureInfo, fontName, klcFilename);
		}
	}
}
