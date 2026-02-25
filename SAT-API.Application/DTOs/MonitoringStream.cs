namespace SAT_API.Application.DTOs;

public class MonitoringStream : Stream
{
    private readonly Stream _baseStream;

    public MonitoringStream(Stream baseStream)
    {
        _baseStream = baseStream;
    }
    
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        // Aquí puedes ver los datos
        Console.WriteLine($"Escribiendo {count} bytes");
        await WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        // Aquí puedes ver los datos
        Console.WriteLine($"Escribiendo {buffer.Length} bytes");
        await _baseStream.WriteAsync(buffer, cancellationToken);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Use WriteAsync instead");
    }

    // Implementar propiedades requeridas
    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _baseStream.Length;
    public override long Position { get; set; }

    public override void Flush() => _baseStream.Flush();
    public override Task FlushAsync(CancellationToken cancellationToken) => _baseStream.FlushAsync(cancellationToken);
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
