// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

#include "StringResources.h"

#include "AudioEmitter.h"

using namespace Bnoerj::Audio;

AudioEmitter::AudioEmitter()
{
	emitterData = new X3DAUDIO_EMITTER();
	ZeroMemory(emitterData, sizeof(X3DAUDIO_EMITTER));

	emitterData->OrientFront.x = XnaVector3::Forward.X;
	emitterData->OrientFront.y = XnaVector3::Forward.Y;
	emitterData->OrientFront.z = -XnaVector3::Forward.Z;

	emitterData->OrientTop.x = XnaVector3::Up.X;
	emitterData->OrientTop.y = XnaVector3::Up.Y;
	emitterData->OrientTop.z = -XnaVector3::Up.Z;

	emitterData->DopplerScaler = 1;
	emitterData->ChannelCount = 1;
	emitterData->ChannelRadius = 1;
	emitterData->CurveDistanceScaler = 1;
}

AudioEmitter::~AudioEmitter()
{
	this->!AudioEmitter();
	GC::SuppressFinalize(this);
}

AudioEmitter::!AudioEmitter()
{
	delete emitterData;
	emitterData = NULL;
}

float AudioEmitter::DopplerScale::get()
{
	return emitterData->DopplerScaler;
}

void AudioEmitter::DopplerScale::set(float value)
{
	if (value < 0)
	{
		throw gcnew ArgumentOutOfRangeException("value", StringResources::InvalidEmitterDopplerScale);
	}

	emitterData->DopplerScaler = value;
}

XnaVector3 AudioEmitter::Position::get()
{
	return XnaVector3(emitterData->Position.x, emitterData->Position.y, -emitterData->Position.z);
}

void AudioEmitter::Position::set(XnaVector3 value)
{
	emitterData->Position.x = value.X;
	emitterData->Position.y = value.Y;
	emitterData->Position.z = -value.Z;
}

XnaVector3 AudioEmitter::Forward::get()
{
	return XnaVector3(emitterData->OrientFront.x, emitterData->OrientFront.y, -emitterData->OrientFront.z);
}

void AudioEmitter::Forward::set(XnaVector3 value)
{
	emitterData->OrientFront.x = value.X;
	emitterData->OrientFront.y = value.Y;
	emitterData->OrientFront.z = -value.Z;
}

XnaVector3 AudioEmitter::Up::get()
{
	return XnaVector3(emitterData->OrientTop.x, emitterData->OrientTop.y, -emitterData->OrientTop.z);
}

void AudioEmitter::Up::set(XnaVector3 value)
{
	emitterData->OrientTop.x = value.X;
	emitterData->OrientTop.y = value.Y;
	emitterData->OrientTop.z = -value.Z;
}

XnaVector3 AudioEmitter::Velocity::get()
{
	return XnaVector3(emitterData->Velocity.x, emitterData->Velocity.y, -emitterData->Velocity.z);
}

void AudioEmitter::Velocity::set(XnaVector3 value)
{
	emitterData->Velocity.x = value.X;
	emitterData->Velocity.y = value.Y;
	emitterData->Velocity.z = -value.Z;
}
