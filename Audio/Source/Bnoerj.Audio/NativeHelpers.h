// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#pragma once

namespace Bnoerj { namespace Native { namespace Helpers {

	using namespace System;
	using namespace System::Runtime::InteropServices;

	ref class StringConverter
	{
	private:
		// Handle for marshalling the unmanged string pointer.
		IntPtr unmanagedStringPointer;

		// Constructor for the managed to native conversion.
		StringConverter(String^ managedString)
		{
			this->unmanagedStringPointer = Marshal::StringToHGlobalAnsi(managedString);
		}

		// Constructor for the managed to native conversion.
		StringConverter(String^ managedString, bool)
		{
			this->unmanagedStringPointer = Marshal::StringToHGlobalUni(managedString);
		}

		// Converts the marshalled pointer to a native string.
		char* ToNativeString()
		{
			return static_cast<char*>(this->unmanagedStringPointer.ToPointer());
		}

		// Converts the marshalled pointer to a native string.
		wchar_t* ToNativeStringUni()
		{
			return static_cast<wchar_t*>(this->unmanagedStringPointer.ToPointer());
		}

	public:
		// Converts a managed string to a native string.
		static char* ToNativeString(String^ managedString)
		{
			return (gcnew StringConverter(managedString))->ToNativeString();
		}

		// Converts a managed string to a native string.
		static wchar_t* ToNativeStringUni(String^ managedString)
		{
			return (gcnew StringConverter(managedString, true))->ToNativeStringUni();
		}

		// Converts a native string to a managed string.
		static String^ ToString(char* nativeString)
		{
			return Marshal::PtrToStringAnsi(static_cast<IntPtr>(nativeString));
		}

		// Converts a native string to a managed string.
		static String^ ToString(wchar_t* nativeString)
		{
			return Marshal::PtrToStringUni(static_cast<IntPtr>(nativeString));
		}

		// Destructor (implicitly implements IDisposable)
		~StringConverter()
		{
			if(this->unmanagedStringPointer != IntPtr::Zero)
			{
				Marshal::FreeHGlobal(this->unmanagedStringPointer);
			}
		}
	};
}}}
