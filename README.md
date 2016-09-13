##C# InfluxDB line protocol writer.##

Line Protocol: see https://docs.influxdata.com/influxdb/v1.0/concepts/glossary/#line-protocol  
Originally based on https://github.com/influxdata/influxdb-csharp - unfortunately I couldn't make the .NET Core project work my ways.

** FEATURES **
- GZIP compression
- Aggregation/Batching of requests (you can specify both a timespan or a number of "points" after which to write to the influx database

**Usage example:**


```csharp
var options = new LineProtocolClientOptions(); // ommitted
BufferedLineProtocolClient lineProtocolClient = new BufferedLineProtocolClient(options);
lineProtocolClient.Enqueue(point);
```