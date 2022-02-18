namespace RedisStreamReader;

using StackExchange.Redis;
using static System.Console;

public static class BlockingReader
{
    public static async Task Listen(
        string connection, 
        string streamName, 
        CancellationToken cancellation,
        Action<Entry> handler)
    {
        // The blocking reader's connection should not be shared with any other operation.
        var redis = ConnectionMultiplexer.Connect(connection);
        if(redis is null)
        {
            WriteLine($"Connection to {connection} failed");
            return;
        }
        WriteLine($"Started consuming from stream {streamName}");

        try
        {
            var db = redis.GetDatabase();

            var currentId = "$"; // listen for new messages
            while(!cancellation.IsCancellationRequested)
            {
                var arguments = new List<object>
                {
                    "BLOCK",
                    "500",
                    "STREAMS",
                    streamName,
                    currentId
                };

                // ExecuteAsync does not take a CancellationToken, so we have to wait the block time
                // before resonding to a cancellation request.
                var result = await db.ExecuteAsync("XREAD", arguments).ConfigureAwait(false);

                if(!result.IsNull)
                {
                    // should only be a single result if querying a single stream
                    foreach (RedisResult[] subresults in (RedisResult[])result)
                    {
                        var name = (RedisValue)subresults[0];
                        foreach(RedisResult[] messages in (RedisResult[])subresults[1])
                        {
                            var id = (RedisValue)messages[0];
                            currentId = id;

                            var nameValuePairs = (RedisResult[])messages[1];
                            var pairs = new Pair[nameValuePairs.Length/2];

                            for(var i = 0; i < nameValuePairs.Length; i+=2)
                            {
                                pairs[i / 2] = new Pair((RedisValue)nameValuePairs[i], (RedisValue)nameValuePairs[i + 1]);
                            }

                            var entry = new Entry(name, id, pairs);
                            handler(entry);
                        }
                    }
                }
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception ex)
        {
            WriteLine(ex.ToString());
        }
        finally
        {
            WriteLine($"Stopped consuming from stream {streamName}");
        }
    }
}

public record Entry(RedisValue StreamName, RedisValue Id, Pair[] Values);

public record Pair(RedisValue Name, RedisValue Value);
