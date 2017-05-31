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
// First of all create a parameters object for the low level InfluxDbLineProtocolClient.
var options = new LineProtocolClientOptions {
    ServerBaseAddress = "http://localhost:8086";
    DatabaseName = "testdb";
    UserName = "root";
    Password = "root";
    UseGzipCompression = true;
};

// Create a new LineProtocolClient
var lineProtocolClient = new LineProtocolClient(options);

// You can now use the lineProtocolClient to write data to InfluxDb.
lineProtocolClient.WriteAsync(...);

// If you want to write to InfluxDb in batches you can wrap the LineProtocolClient in a BufferedLineProtocolClient. 
// In this example the buffer will grow to a maximum of 15.000 items -OR- gather items for a maximum time of 3 seconds - whichever threshold is reached first.
BufferedLineProtocolClient bufferedLineProtocolClient = new BufferedLineProtocolClient(lineProtocolClient, maximumMeasurementPoints: 15000, maximumElapsedTime: TimeSpan.FromSeconds(2);

// You can the enqueue new data
bufferedLineProtocolClient.Enqueue(point1);
bufferedLineProtocolClient.Enqueue(point2);
bufferedLineProtocolClient.Enqueue(point3);
```