using System;
using System.IO;

namespace NetP3DLib.IO;
public sealed class BoundedStream : Stream
{
    private readonly Stream _baseStream;
    private readonly long _start;
    private readonly long _length;

    public BoundedStream(Stream baseStream, long length)
    {
        _baseStream = baseStream;
        _start = baseStream.Position;
        _length = length;
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => false;

    public override long Length => _length;

    public override long Position
    {
        get => _baseStream.Position - _start;
        set
        {
            if (value < 0 || value > _length)
                throw new IOException("Attempt to seek outside bounded region.");

            _baseStream.Position = _start + value;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        long remaining = _length - Position;
        if (remaining <= 0)
            return 0;

        if (count > remaining)
            count = (int)remaining;

        return _baseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long target = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => _length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        if (target < 0 || target > _length)
            throw new IOException("Attempt to seek outside bounded region.");

        return Position = target;
    }

    public override void Flush() { }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}