using NetP3DLib.P3D.Extensions;
using System.IO;

namespace NetP3DLib.P3D.Types;

internal class P3DString
{
    private readonly Chunk _chunk;
    private string _value;
    public string Value
    {
        get => _value;
        set
        {
            if (_value == value)
                return;

            var oldSize = _chunk.HeaderSize;
            _value = value;
            _chunk.RecalculateSize(oldSize);
        }
    }

    internal P3DString(Chunk chunk, BinaryReader br)
    {
        _chunk = chunk;
        _value = br.ReadP3DString();
    }

    internal P3DString(Chunk chunk, string value)
    {
        _chunk = chunk;
        _value = value;
    }
}
