using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EchoServer;  // <-- простір імен основного проекту

public class EchoServerTests
{
    [Fact]
    public async Task HandleClientAsync_ShouldEchoData()
    {
        var logger = new TestLogger();
        var server = new EchoServer(5000, logger);

        var input = "hello!";
        var bytes = Encoding.UTF8.GetBytes(input);

        var stream = new MemoryStream();
        await stream.WriteAsync(bytes, 0, bytes.Length);
        stream.Position = 0;

        await server.HandleClientAsync(stream, CancellationToken.None);

        var output = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Equal(input, output);
    }

    [Fact]
    public async Task HandleClientAsync_EmptyStream_NoError()
    {
        var logger = new TestLogger();
        var server = new EchoServer(5000, logger);

        var stream = new MemoryStream();

        await server.HandleClientAsync(stream, CancellationToken.None);

        Assert.Empty(stream.ToArray());
    }

    [Fact]
    public async Task HandleClientAsync_ShouldWriteLog()
    {
        var logger = new TestLoggerWithMessages();
        var server = new EchoServer(5000, logger);

        var bytes = Encoding.UTF8.GetBytes("ping");
        var stream = new MemoryStream(bytes);
        stream.Position = 0;

        await server.HandleClientAsync(stream, CancellationToken.None);

        Assert.True(logger.Messages.Count > 0);
    }

    private class TestLogger : ILogger
    {
        public void Log(string message) { }
    }

    private class TestLoggerWithMessages : ILogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }
}
