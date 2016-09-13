##C# InfluxDB line protocol writer.##

[Official InfluxDB Line Protocol documentation](https://docs.influxdata.com/influxdb/v1.0/write_protocols/line/)  
This code is based on [influxdb-csharp](https://github.com/influxdata/influxdb-csharp)  

**FEATURES**
- GZIP compression which significantly reduces the amout of data sent over the wire
- Aggregation/Batching of write requests  
  The batching/aggregating is triggered by whatever event occurs first.  
  In the example below either after 15.000 LineProtocolPoints -or- after 3 seconds.


**Usage example:**

```csharp
var options = new LineProtocolClientOptions {
    ServerBaseAddress = "http://localhost:8086";
    DatabaseName = "testdb";
    UserName = "root";
    Password = "root";
    UseGzipCompression = true;
    RequestAggregationMaxCount = 15000; // maximum number of LineProtocolPoints in one batch
    RequestAggregationMaxSeconds = 3; // maximum seconds after which a batch is written
};

BufferedLineProtocolClient lineProtocolClient = new BufferedLineProtocolClient(options);

// enqueue LineProtocolPoint
lineProtocolClient.Enqueue(point1);
lineProtocolClient.Enqueue(point2);
lineProtocolClient.Enqueue(point3);
```
