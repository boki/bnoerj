// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Bnoerj.Locales.KeyboardLayouts;

namespace Bnoerj.Locales.Importer
{
	internal static class NativeMethods
	{
		internal static class Gdi32
		{
			[Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			internal struct TEXTMETRIC
			{
				public int tmHeight;
				public int tmAscent;
				public int tmDescent;
				public int tmInternalLeading;
				public int tmExternalLeading;
				public int tmAveCharWidth;
				public int tmMaxCharWidth;
				public int tmWeight;
				public int tmOverhang;
				public int tmDigitizedAspectX;
				public int tmDigitizedAspectY;
				public char tmFirstChar;            // this is why we use CharSet.Unicode in StructLayoutAttribute
				public char tmLastChar;
				public char tmDefaultChar;
				public char tmBreakChar;
				public byte tmItalic;
				public byte tmUnderlined;
				public byte tmStruckOut;
				public byte tmPitchAndFamily;
				public byte tmCharSet;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct OUTLINETEXTMETRIC
			{
				public uint otmSize;
				public TEXTMETRIC otmTextMetrics;
				public byte otmFiller;
				public PANOSE otmPanoseNumber;
				public uint otmfsSelection;
				public uint otmfsType;
				public int otmsCharSlopeRise;
				public int otmsCharSlopeRun;
				public int otmItalicAngle;
				public uint otmEMSquare;
				public int otmAscent;
				public int otmDescent;
				public uint otmLineGap;
				public uint otmsCapEmHeight;
				public uint otmsXHeight;
				public RECT otmrcFontBox;
				public int otmMacAscent;
				public int otmMacDescent;
				public uint otmMacLineGap;
				public uint otmusMinimumPPEM;
				public POINT otmptSubscriptSize;
				public POINT otmptSubscriptOffset;
				public POINT otmptSuperscriptSize;
				public POINT otmptSuperscriptOffset;
				public uint otmsStrikeoutSize;
				public int otmsStrikeoutPosition;
				public int otmsUnderscoreSize;
				public int otmsUnderscorePosition;
				public uint otmpFamilyName;
				public uint otmpFaceName;
				public uint otmpStyleName;
				public uint otmpFullName;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct PANOSE
			{
				public byte bFamilyType;
				public byte bSerifStyle;
				public byte bWeight;
				public byte bProportion;
				public byte bContrast;
				public byte bStrokeVariation;
				public byte bArmStyle;
				public byte bLetterform;
				public byte bMidline;
				public byte bXHeight;
			}

			[Serializable, StructLayout(LayoutKind.Sequential)]
			internal struct RECT
			{
				public int Left;
				public int Top;
				public int Right;
				public int Bottom;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct POINT
			{
				public int X;
				public int Y;
			}

			[StructLayout(LayoutKind.Sequential)]
			internal struct KERNINGPAIR
			{
				internal ushort wFirst;
				internal ushort wSecond;
				internal int iKernAmount;
			}

			[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetOutlineTextMetricsW", ExactSpelling = true, SetLastError = true)]
			internal static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr lpOTM);
			//internal static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, [Out] OUTLINETEXTMETRIC lpOTM);

			[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetKerningPairsW", ExactSpelling = true)]
			internal static extern uint GetKerningPairs(IntPtr hdc, uint nNumPairs, [Out, MarshalAs(UnmanagedType.LPArray)] KERNINGPAIR[] lpkrnpair);

			[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "SelectObject", ExactSpelling = true)]
			internal static extern IntPtr SelectObject([In] IntPtr hDC, [In] IntPtr hgdiObject);
		}

		internal static class Shlwapi
		{
			[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			internal static extern uint SHLoadIndirectString(String pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);
		}

		internal static class User32
		{
			internal const uint KLF_NOTELLSHELL = 0x00000080;

			[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
			internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadKeyboardLayoutW", ExactSpelling = true)]
			internal static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

			[DllImport("user32.dll", ExactSpelling = true)]
			internal static extern bool UnloadKeyboardLayout(IntPtr hkl);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			internal static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, KeysEx[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

			[DllImport("user32.dll", ExactSpelling = true)]
			internal static extern int GetKeyboardLayoutList(int nBuff, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] lpList);
		}
	}
}
