namespace RedisStreamReader;

using StackExchange.Redis;
using static System.Console;

public static class Publisher
{
    public static async Task Start(string connection, string streamName, CancellationToken cancellation)
    {
        var redis = ConnectionMultiplexer.Connect(connection);
        if(redis is null)
        {
            WriteLine($"Connection to {connection} failed");
            return;
        }

        var db = redis.GetDatabase();

        try
        {
            var count = 0;
            while (!cancellation.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellation);

                var values = new NameValueEntry[]
                {
                    new ("one", $"one at {count}"),
                    new ("two", $"two at {count}")
                };

                var key = await db.StreamAddAsync(streamName, values);

                WriteLine($"{count} => {key}");
                count++;
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception exception)
        {
            WriteLine(exception.ToString());
        }
    }
}
