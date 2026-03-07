using NetP3DLib.P3D.Extensions;
using System.IO;

namespace NetP3DLib.P3D.Types;

internal class FourCC
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

    internal FourCC(Chunk chunk, BinaryReader br)
    {
        _chunk = chunk;
        _value = br.ReadFourCC();
    }

    internal FourCC(Chunk chunk, string value)
    {
        _chunk = chunk;
        _value = value;
    }
}
