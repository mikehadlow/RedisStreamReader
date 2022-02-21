# RedisStreamReader

This supports the blog post [Blocking XREAD From A Redis Stream Using StackExchange.Redis](https://mikehadlow.com/posts/xread-from-a-redis-stream-using-stackexchange-redis/) on [mikehadlow.com](https://mikehadlow.com/)

An example of how to XREAD from a Redis stream using the [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) nuget package.

This is a single .NET6 console project in a standard VS 2022 solution. Simply clone, build and run with F5 from VS.

The `Publisher` class adds messages to a stream. 
The `BlockingReader` class demonstrates using `XREAD` do execute a blocking read from the stream.

Running the console application should give the following output (ctrl-C to exit):
```
Starting fixture stream reading.
Started consuming from stream RSR:test
0 => 1645461798160-0
        Id: 1645461798160-0
                one: one at 0
                two: two at 0
1 => 1645461799174-0
        Id: 1645461799174-0
                one: one at 1
                two: two at 1
2 => 1645461800178-0
        Id: 1645461800178-0
                one: one at 2
                two: two at 2
3 => 1645461801191-0
        Id: 1645461801191-0
                one: one at 3
                two: two at 3
Stopped consuming from stream RSR:test
```