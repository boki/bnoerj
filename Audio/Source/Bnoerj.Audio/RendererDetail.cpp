// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"
#include "RendererDetail.h"

using namespace Bnoerj::Audio;

RendererDetail::RendererDetail(String^ friendlyName, Guid guid)
	: friendlyName(friendlyName)
	, guid(guid)
{}

String^ RendererDetail::FriendlyName::get()
{
	return friendlyName;
}

Guid RendererDetail::RendererId::get()
{
	return guid;
}

bool RendererDetail::Equals(Object^ other)
{
	if (other == nullptr || dynamic_cast<RendererDetail^>(other) == nullptr)
	{
		return false;
	}
	return this == static_cast<RendererDetail^>(other);
}

int RendererDetail::GetHashCode()
{
	int hash = String::IsNullOrEmpty(friendlyName) == false ? friendlyName->GetHashCode() : 0;
	return hash ^ guid.GetHashCode();
}

bool RendererDetail::operator ==(RendererDetail^ a, RendererDetail^ b)
{
	return a->friendlyName == b->friendlyName && a->guid == b->guid;
}

bool RendererDetail::operator !=(RendererDetail^ a, RendererDetail^ b)
{
	return !(a == b);
}
