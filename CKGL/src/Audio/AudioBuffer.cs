﻿#nullable enable

using System;
using System.IO;
using static OpenAL.Bindings;
using static SDL2.SDL;

namespace CKGL
{
	public class AudioBuffer
	{
		internal uint ID;

		public AudioBuffer(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException(file);

			ID = alGenBuffer();
			Audio.CheckALError("Could not create Buffer");

			SDL_AudioSpec wavspec = new SDL_AudioSpec();
			SDL_LoadWAV(file, ref wavspec, out IntPtr audioBuffer, out uint audioLength);

			// map wav header to openal format
			alBufferFormat format;
			switch (wavspec.format)
			{
				case AUDIO_U8:
				case AUDIO_S8:
					format = wavspec.channels == 2 ? alBufferFormat.Stereo8 : alBufferFormat.Mono8;
					break;
				case AUDIO_U16:
				case AUDIO_S16:
					format = wavspec.channels == 2 ? alBufferFormat.Stereo16 : alBufferFormat.Mono16;
					break;
				default:
					SDL_FreeWAV(audioBuffer);
					throw new CKGLException($"SDL failed parsing wav: {file}");
			}

			alBufferData(ID, format, audioBuffer, (int)audioLength, wavspec.freq);
			Audio.CheckALError("Could not set Buffer Data");

			SDL_FreeWAV(audioBuffer);

			Audio.Buffers.Add(this);
		}

		public void Destroy()
		{
			alDeleteBuffer(ID);
			Audio.CheckALError("Could not destroy Effect");

			Audio.Buffers.Remove(this);
		}

		public void Play(AudioFilter? directFilter = null,
			(AudioEffect? effect, AudioFilter? filter)? send1 = null,
			(AudioEffect? effect, AudioFilter? filter)? send2 = null,
			(AudioEffect? effect, AudioFilter? filter)? send3 = null,
			(AudioEffect? effect, AudioFilter? filter)? send4 = null)
		{
			AudioSource audioSource = new AudioSource();
			audioSource.DestroyOnStop = true;
			audioSource.AudioBuffer = this;
			if (send1 != null)
				audioSource.Send1 = send1;
			if (send2 != null)
				audioSource.Send2 = send2;
			if (send3 != null)
				audioSource.Send3 = send3;
			if (send4 != null)
				audioSource.Send4 = send4;
			if (directFilter != null)
				audioSource.DirectFilter = directFilter;
			audioSource.Play();
		}
	}
}