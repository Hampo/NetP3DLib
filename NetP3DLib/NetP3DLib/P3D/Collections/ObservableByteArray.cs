using System;

namespace NetP3DLib.P3D.Collections;

public class ObservableByteArray
{
    private readonly byte[] _data;
    private readonly Action? _onChanged;

    public ObservableByteArray(int size, Action? onChanged = null)
    {
        _data = new byte[size];
        _onChanged = onChanged;
    }

    public ObservableByteArray(byte[] data, Action? onChanged = null)
    {
        _data = data ?? [];
        _onChanged = onChanged;
    }

    public int Length => _data.Length;

    public byte this[int index]
    {
        get => _data[index];
        set
        {
            if (_data[index] != value)
            {
                _data[index] = value;
                _onChanged?.Invoke();
            }
        }
    }

    public byte[] ToArray() => (byte[])_data.Clone();
}