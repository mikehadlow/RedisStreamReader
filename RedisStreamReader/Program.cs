namespace RedisStreamReader;

using static System.Console;

public static class Program
{
    private const string connection = "localhost";
    private const string stream = "RSR:test";

    public static Task Main()
    {
        var cts = new CancellationTokenSource();

        CancelKeyPress += (_, args) => 
        {
            cts.Cancel();
            cts.Dispose();
            args.Cancel = true;
        };

        WriteLine("Starting fixture stream reading.");

        var readerTask = BlockingReader.Listen(
            connection, 
            stream, 
            cts.Token,
            entry => 
            {
                WriteLine($"\tId: {entry.Id}");
                foreach(var pair in entry.Values)
                {
                    WriteLine($"\t\t{pair.Name}: {pair.Value}");
                }
            });

        var publisherTask = Publisher.Start(connection, stream, cts.Token);

        return Task.WhenAll(readerTask, publisherTask);
    }


}
