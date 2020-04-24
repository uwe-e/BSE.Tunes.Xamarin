﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AudioToolbox;
using Foundation;
using UIKit;

namespace BSE.Tunes.XApp.iOS
{
	internal class AudioBuffer
	{
		public IntPtr Buffer { get; set; }

		public List<AudioStreamPacketDescription> PacketDescriptions { get; set; }

		public int CurrentOffset { get; set; }

		public bool IsInUse { get; set; }
	}

	public class StreamingPlayback : IDisposable
	{
		public event EventHandler Finished;
		public event Action<OutputAudioQueue> OutputReady;

		// the AudioToolbox decoder
		private AudioFileStream _audioFileStream;
		private int _bufferSize = 128 * 1024;
		private List<AudioBuffer> _outputBuffers;
		private AudioBuffer _currentBuffer;
		// Maximum buffers
		private int _maxBufferCount = 4;
		// Keep track of all queued up buffers, so that we know that the playback finished
		private int _queuedBufferCount = 0;
		// Current Filestream Position - if we don't keep track we don't know when to push the last uncompleted buffer
		private long _currentByteCount = 0;
		//Used to trigger a dump of the last buffer.
		private bool _lastPacket;

		public OutputAudioQueue OutputQueue;

		public bool Started { get; private set; }

		public float Volume
		{
			get
			{
				return OutputQueue.Volume;
			}

			set
			{
				OutputQueue.Volume = value;
			}
		}

		/// <summary>
		/// Defines the size forearch buffer, when using a slow source use more buffers with lower buffersizes
		/// </summary>
		public int BufferSize
		{
			get
			{
				return _bufferSize;
			}

			set
			{
				_bufferSize = value;
			}
		}

		/// <summary>
		/// Defines the maximum Number of Buffers to use, the count can only change after Reset is called or the
		/// StreamingPlayback is freshly instantiated
		/// </summary>
		public int MaxBufferCount
		{
			get
			{
				return _maxBufferCount;
			}

			set
			{
				_maxBufferCount = value;
			}
		}

		public StreamingPlayback() : this(AudioFileType.MP3)
		{
		}

		public StreamingPlayback(AudioFileType type)
		{
			_audioFileStream = new AudioFileStream(type);
			_audioFileStream.PacketDecoded += AudioPacketDecoded;
			_audioFileStream.PropertyFound += AudioPropertyFound;
		}

		public void Reset()
		{
			if (_audioFileStream != null)
			{
				_audioFileStream.Close();
				_audioFileStream = new AudioFileStream(AudioFileType.MP3);
				_currentByteCount = 0;
				_audioFileStream.PacketDecoded += AudioPacketDecoded;
				_audioFileStream.PropertyFound += AudioPropertyFound;
			}
		}

		public void ResetOutputQueue()
		{
			if (OutputQueue != null)
			{
				OutputQueue.Stop(true);
				OutputQueue.Reset();
				foreach (AudioBuffer buf in _outputBuffers)
				{
					buf.PacketDescriptions.Clear();
					OutputQueue.FreeBuffer(buf.Buffer);
				}
				_outputBuffers = null;
				OutputQueue.Dispose();
			}
		}

		/// <summary>
		/// Stops the OutputQueue
		/// </summary>
		public void Pause()
		{
			OutputQueue.Pause();
			Started = false;
		}

		/// <summary>
		/// Starts the OutputQueue
		/// </summary>
		public void Play()
		{
			OutputQueue.Start();
			Started = true;
		}

		/// <summary>
		/// Main methode to kick off the streaming, just send the bytes to this method
		/// </summary>
		public void ParseBytes(byte[] buffer, int count, bool discontinuity, bool lastPacket)
		{
			this._lastPacket = lastPacket;
			_audioFileStream.ParseBytes(buffer, 0, count, discontinuity);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Cleaning up all the native Resource
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{

				if (OutputQueue != null)
					OutputQueue.Stop(true);

				if (_outputBuffers != null)
				{
					foreach (var b in _outputBuffers)
						OutputQueue.FreeBuffer(b.Buffer);

					_outputBuffers.Clear();
					_outputBuffers = null;
				}

				if (_audioFileStream != null)
				{
					_audioFileStream.Close();
					_audioFileStream = null;
				}

				if (OutputQueue != null)
				{
					OutputQueue.Dispose();
					OutputQueue = null;
				}
			}
		}

		/// <summary>
		/// Saving the decoded Packets to our active Buffer, if the Buffer is full queue it into the OutputQueue
		/// and wait until another buffer gets freed up
		/// </summary>
		void AudioPacketDecoded(object sender, PacketReceivedEventArgs args)
		{
			foreach (var p in args.PacketDescriptions)
			{
				_currentByteCount += p.DataByteSize;

				AudioStreamPacketDescription pd = p;

				int left = _bufferSize - _currentBuffer.CurrentOffset;
				if (left < pd.DataByteSize)
				{
					EnqueueBuffer();
					WaitForBuffer();
				}

				AudioQueue.FillAudioData(_currentBuffer.Buffer, _currentBuffer.CurrentOffset, args.InputData, (int)pd.StartOffset, pd.DataByteSize);
				// Set new offset for this packet
				pd.StartOffset = _currentBuffer.CurrentOffset;
				// Add the packet to our Buffer
				_currentBuffer.PacketDescriptions.Add(pd);
				// Add the Size so that we know how much is in the buffer
				_currentBuffer.CurrentOffset += pd.DataByteSize;
			}

			if ((_audioFileStream != null && _currentByteCount == _audioFileStream.DataByteCount) || _lastPacket)
				EnqueueBuffer();
		}

		/// <summary>
		/// Flush the current buffer and close the whole thing up
		/// </summary>
		public void FlushAndClose()
		{
			if (OutputQueue != null)
			{
				EnqueueBuffer();
				OutputQueue.Flush();
			}

			Dispose();
		}

		/// <summary>
		/// Enqueue the active buffer to the OutputQueue
		/// </summary>
		void EnqueueBuffer()
		{
			_currentBuffer.IsInUse = true;
			OutputQueue.EnqueueBuffer(_currentBuffer.Buffer, _currentBuffer.CurrentOffset, _currentBuffer.PacketDescriptions.ToArray());
			_queuedBufferCount++;
			StartQueueIfNeeded();
		}

		/// <summary>
		/// Wait until a buffer is freed up
		/// </summary>
		void WaitForBuffer()
		{
			int curIndex = _outputBuffers.IndexOf(_currentBuffer);
			_currentBuffer = _outputBuffers[curIndex < _outputBuffers.Count - 1 ? curIndex + 1 : 0];

			lock (_currentBuffer)
			{
				while (_currentBuffer.IsInUse)
					Monitor.Wait(_currentBuffer);
			}
		}

		void StartQueueIfNeeded()
		{
			if (Started)
				return;

			Play();
		}

		/// <summary>
		/// When a AudioProperty in the fed packets is found this callback is called
		/// </summary>
		void AudioPropertyFound(object sender, PropertyFoundEventArgs args)
		{
			if (args.Property == AudioFileStreamProperty.BitRate)
			{

			}
			if (args.Property == AudioFileStreamProperty.ReadyToProducePackets)
			{
				Started = false;

				if (OutputQueue != null)
					OutputQueue.Dispose();

				OutputQueue = new OutputAudioQueue(_audioFileStream.StreamBasicDescription);
				if (OutputReady != null)
					OutputReady(OutputQueue);

				_currentByteCount = 0;
				OutputQueue.BufferCompleted += HandleBufferCompleted;
				_outputBuffers = new List<AudioBuffer>();

				for (int i = 0; i < MaxBufferCount; i++)
				{
					IntPtr outBuffer;
					OutputQueue.AllocateBuffer(BufferSize, out outBuffer);
					_outputBuffers.Add(new AudioBuffer()
					{
						Buffer = outBuffer,
						PacketDescriptions = new List<AudioStreamPacketDescription>()
					});
				}

				_currentBuffer = _outputBuffers.First();

				OutputQueue.MagicCookie = _audioFileStream.MagicCookie;
			}
		}

		/// <summary>
		/// Is called when a buffer is completly read and can be freed up
		/// </summary>
		void HandleBufferCompleted(object sender, BufferCompletedEventArgs e)
		{
			_queuedBufferCount--;
			IntPtr buf = e.IntPtrBuffer;

			foreach (var buffer in _outputBuffers)
			{
				if (buffer.Buffer != buf)
					continue;

				// free Buffer
				buffer.PacketDescriptions.Clear();
				buffer.CurrentOffset = 0;
				lock (buffer)
				{
					buffer.IsInUse = false;
					Monitor.Pulse(buffer);
				}
			}

			if (_queuedBufferCount == 0 && Finished != null)
				Finished(this, new EventArgs());
		}
	}
}