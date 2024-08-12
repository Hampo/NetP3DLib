using System.IO;

namespace NetP3DLib.P3D.Chunks;

[ChunkAttributes((uint)ChunkIdentifier.Old_Scenegraph_Root)]
public class OldScenegraphRootChunk : Chunk
{
    public override byte[] DataBytes
    {
        get
        {
            return [];
        }
    }
    public override uint DataLength => 0;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for ChunkLoader.")]
    public OldScenegraphRootChunk(BinaryReader br) : base((uint)ChunkIdentifier.Old_Scenegraph_Root)
    { }

    public OldScenegraphRootChunk() : base((uint)ChunkIdentifier.Old_Scenegraph_Root)
    { }

    public override void Validate()
    {
        base.Validate();
    }

    internal override void WriteData(BinaryWriter bw)
    { }
}