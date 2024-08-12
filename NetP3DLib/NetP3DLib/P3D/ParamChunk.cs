using System.IO;

namespace NetP3DLib.P3D;

public abstract class ParamChunk : Chunk
{
    public string Param { get; set; }

    public ParamChunk(uint ID) : base(ID) { }

    public override void Validate()
    {
        if (Param == null || Param.Length == 0)
            throw new InvalidDataException($"{nameof(Param)} must be at least 1 char.");

        if (Param.Length > 4)
            throw new InvalidDataException($"The max length of {nameof(Param)} is 4 chars.");

        base.Validate();
    }
}
