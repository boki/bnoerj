// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Bnoerj.Locales.KeyboardLayouts;
using Bnoerj.Locales.Importer.KeyboardLayouts;

namespace Bnoerj.Locales.Importer
{
	internal class TestWriter : BinaryWriter
	{
		public override void Write(bool value)
		{
			System.Diagnostics.Debug.WriteLine(value);
		}
		public override void Write(int value)
		{
			System.Diagnostics.Debug.WriteLine(value);
		}
		public override void Write(uint value)
		{
			System.Diagnostics.Debug.WriteLine(value);
		}
		public override void Write(String value)
		{
			System.Diagnostics.Debug.WriteLine(value);
		}

		public void WriteObject<T>(T value)
		{
			System.Diagnostics.Debug.WriteLine(value.ToString());
		}
	}

	public class Temp
	{
		public static void TestImport(int localeId)
		{
			String lid = String.Format("{0:x8}", localeId);
			KeyboardLayoutSource kls = new KeyboardLayoutSource(lid);
			KeyboardLayoutProcessor klp = new KeyboardLayoutProcessor();
			KeyboardLayoutContent klc = klp.Process(kls, null);
			VirtualKey vk = klc.Keys[(int)KeysEx.VK_Z].VirtualKey;
			byte[] buffer = new byte[1024];
			MemoryStream ms = new MemoryStream(buffer);
			BinaryWriter bw = new BinaryWriter(ms);
			foreach (VirtualKeyValue value in vk.ShiftStates)
			{
				bw.Write(value.Characters);
				System.Diagnostics.Debug.WriteLine(value.Characters);
			}
			bw.Close();
		}

		public static void DumpScanCodesVirtualKeys(String kid)
		{
			IntPtr hkl = NativeMethods.User32.LoadKeyboardLayout(kid, NativeMethods.User32.KLF_NOTELLSHELL);
			if (hkl == IntPtr.Zero)
			{
				Console.WriteLine("Sorry, that keyboard does not seem to be valid.");
			}
			else
			{
				int nLCID = int.Parse(kid, NumberStyles.HexNumber);
				CultureInfo ci = new System.Globalization.CultureInfo(nLCID & 0xFFFF);
				Console.WriteLine("{0}, {1}, {2}\n", ci.Name, ci.DisplayName, ci.EnglishName);

				Console.WriteLine("VK\tSC");
				Console.WriteLine("------------");
				for (KeysEx vk = KeysEx.None; vk <= KeysEx.Last; vk++)
				{
					uint sc = NativeMethods.User32.MapVirtualKeyEx((uint)vk, 0, hkl);
					Console.WriteLine("{0}\t{1}", vk, sc);
				}

				Console.WriteLine("\nSC\tVK");
				Console.WriteLine("------------");
				for (KeysEx vk = KeysEx.None; vk <= KeysEx.Last; vk++)
				{
					uint sc = NativeMethods.User32.MapVirtualKeyEx((uint)vk, 0, hkl);
					Console.WriteLine("{1}\t{0}", vk, sc);
				}

				InputLanguageCollection ilc = InputLanguage.InstalledInputLanguages;
				foreach (InputLanguage il in ilc)
				{
					if (hkl == il.Handle)
					{
						hkl = IntPtr.Zero;
						break;
					}
				}
				if (hkl != IntPtr.Zero)
				{
					NativeMethods.User32.UnloadKeyboardLayout(hkl);
				}
			}
		}
	}
}
