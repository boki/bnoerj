// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"
#include "AudioListener.h"

using namespace Bnoerj::Audio;

AudioListener::AudioListener()
{
	listenerData = new X3DAUDIO_LISTENER();
	ZeroMemory(listenerData, sizeof(X3DAUDIO_LISTENER));

	listenerData->OrientFront.x = XnaVector3::Forward.X;
	listenerData->OrientFront.y = XnaVector3::Forward.Y;
	listenerData->OrientFront.z = -XnaVector3::Forward.Z;

	listenerData->OrientTop.x = XnaVector3::Up.X;
	listenerData->OrientTop.y = XnaVector3::Up.Y;
	listenerData->OrientTop.z = -XnaVector3::Up.Z;
}

AudioListener::~AudioListener()
{
	this->!AudioListener();
	GC::SuppressFinalize(this);
}

AudioListener::!AudioListener()
{
	delete listenerData;
	listenerData = NULL;
}

XnaVector3 AudioListener::Position::get()
{
	return XnaVector3(listenerData->Position.x, listenerData->Position.y, -listenerData->Position.z);
}

void AudioListener::Position::set(XnaVector3 value)
{
	listenerData->Position.x = value.X;
	listenerData->Position.y = value.Y;
	listenerData->Position.z = -value.Z;
}

XnaVector3 AudioListener::Forward::get()
{
	return XnaVector3(listenerData->OrientFront.x, listenerData->OrientFront.y, -listenerData->OrientFront.z);
}

void AudioListener::Forward::set(XnaVector3 value)
{
	listenerData->OrientFront.x = value.X;
	listenerData->OrientFront.y = value.Y;
	listenerData->OrientFront.z = -value.Z;
}

XnaVector3 AudioListener::Up::get()
{
	return XnaVector3(listenerData->OrientTop.x, listenerData->OrientTop.y, -listenerData->OrientTop.z);
}

void AudioListener::Up::set(XnaVector3 value)
{
	listenerData->OrientTop.x = value.X;
	listenerData->OrientTop.y = value.Y;
	listenerData->OrientTop.z = -value.Z;
}

XnaVector3 AudioListener::Velocity::get()
{
	return XnaVector3(listenerData->Velocity.x, listenerData->Velocity.y, -listenerData->Velocity.z);
}

void AudioListener::Velocity::set(XnaVector3 value)
{
	listenerData->Velocity.x = value.X;
	listenerData->Velocity.y = value.Y;
	listenerData->Velocity.z = -value.Z;
}
