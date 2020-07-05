using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BSE.Tunes.XApp.iOS.IO
{
    public class QueueStream : Stream
	{
		Stream _writeStream;
		Stream _readStream;
		long _size;
		bool _isDone;
		object _pLock = new object();

		public QueueStream(string storage)
		{
			_writeStream = new FileStream(storage, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096);
			_readStream = new FileStream(storage, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096);
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { return _readStream.Length; }
		}

		public override long Position
		{
			get { return _readStream.Position; }
			set { throw new NotImplementedException(); }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			lock (_pLock)
			{
				while (true)
				{
					if (Position < _size)
					{
						int n = _readStream.Read(buffer, offset, count);
						return n;
					}
					else if (_isDone)
						return 0;

					try
					{
						Debug.WriteLine("Waiting for data");
						Monitor.Wait(_pLock);
						Debug.WriteLine("Waking up, data available");
					}
					catch
					{
					}
				}
			}
		}

		public void Push(byte[] buffer, int offset, int count)
		{
			lock (_pLock)
			{
				_writeStream.Write(buffer, offset, count);
				_size += count;
				_writeStream.Flush();
				Monitor.Pulse(_pLock);
			}
		}

		public void Done()
		{
			lock (_pLock)
			{
				Monitor.Pulse(_pLock);
				_isDone = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_readStream.Close();
				_readStream.Dispose();
				_writeStream.Close();
				_writeStream.Dispose();
			}
			base.Dispose(disposing);
		}

		#region non implemented abstract members of Stream

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		#endregion

	}
}