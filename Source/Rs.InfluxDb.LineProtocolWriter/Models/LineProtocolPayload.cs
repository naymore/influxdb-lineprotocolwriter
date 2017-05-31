using System;
using System.Collections.Generic;
using System.IO;

namespace Rs.InfluxDb.LineProtocolWriter.Models
{
    public class LineProtocolPayload
    {
        private readonly List<LineProtocolPoint> _points = new List<LineProtocolPoint>();

        public LineProtocolPayload()
        {
            // NOP
        }

        public int Count => _points.Count;

        public void Add(LineProtocolPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            _points.Add(point);
        }

        public void AddRange(IEnumerable<LineProtocolPoint> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            _points.AddRange(points);
        }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));

            foreach (LineProtocolPoint point in _points)
            {
                point.Render(textWriter);
                textWriter.Write("\n");
            }
        }
    }
}