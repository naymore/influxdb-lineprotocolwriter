using System;
using Rs.InfluxDb.LineProtocolWriter.Models;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rs.InfluxDb.LineProtocolWriter.Exceptions;

namespace Rs.InfluxDb.LineProtocolWriter
{
    public class BufferedLineProtocolClient : IDisposable
    {
        private readonly ISubject<LineProtocolPoint> _syncedSubject;
        private readonly IDisposable _subscription;
        private readonly LineProtocolClient _lineProtocolClient;

        private bool _disposedValue;

        public BufferedLineProtocolClient(LineProtocolClientOptions lineProtocolClientOptions)
        {
            _lineProtocolClient = new LineProtocolClient(lineProtocolClientOptions);

            Subject<LineProtocolPoint> subject = new Subject<LineProtocolPoint>();
            _syncedSubject = Subject.Synchronize(subject);

            TimeSpan requestAggegationTimer = TimeSpan.FromSeconds(lineProtocolClientOptions.RequestAggregationMaxSeconds);

            _subscription = _syncedSubject
                .Buffer(requestAggegationTimer, lineProtocolClientOptions.RequestAggregationMaxCount)
                .Subscribe(onNext: async (pointsList) => await WriteToInflux(pointsList));
        }

        private async Task WriteToInflux(IList<LineProtocolPoint> pointsList)
        {
            if (pointsList == null || pointsList.Count == 0)
                return;

            LineProtocolPayload payload = new LineProtocolPayload();
            payload.AddRange(pointsList);

            var result = await _lineProtocolClient.WriteAsync(payload);

            if (!result.Success)
            {
                throw new LineProtocolException(result.ErrorMessage);
            }
        }

        public void Enqueue(LineProtocolPoint point)
        {
            _syncedSubject.OnNext(point);
        }

        #region IDisposable Support
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription?.Dispose();
                    _lineProtocolClient.Dispose();
                }
                
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}