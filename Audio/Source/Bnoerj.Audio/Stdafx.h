// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#define _WIN32_DCOM
#define _CRT_SECURE_NO_DEPRECATE
#include <windows.h>
#pragma warning(push)
#pragma warning(disable: 4793) // xact3wb.h(130): '__asm': causes native code generation for function 'void XACTWaveBank::SwapBytes(DWORD &)'
#include <xact3.h>
#pragma warning(pop)
#include <xact3d3.h>
#pragma warning(push)
#pragma warning(disable: 4091) // lock.h(51): ignored on left of 'int' when no variable is decalred
#include <msclr/lock.h>
#pragma warning(pop)
#pragma warning(push)
#pragma warning(disable: 4400) // ptr.h(43): 'const msclr::_detail::smart_ptr<_interface_type> %': const/volatile qualifiers on this type are not supported
//#include <msclr/com/ptr.h>
#pragma warning(pop)

typedef Microsoft::Xna::Framework::Vector3 XnaVector3;
