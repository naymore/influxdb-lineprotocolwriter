using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rs.InfluxDb.LineProtocolWriter.Models
{
    public class LineProtocolPoint
    {
        private readonly string _measurement;
        private IReadOnlyDictionary<string, object> _fields;
        private IReadOnlyDictionary<string, string> _tags;
        private DateTime? _utcTimestamp;
        
        public LineProtocolPoint(string measurement, IReadOnlyDictionary<string, object> fields, IReadOnlyDictionary<string, string> tags = null, DateTime? utcTimestamp = null)
        {
            if (string.IsNullOrEmpty(measurement))
                throw new ArgumentException("A measurement name must be specified");

            if (fields == null || fields.Count == 0)
                throw new ArgumentException("At least one field must be specified");

            foreach (KeyValuePair<string, object> field in fields)
            {
                if (string.IsNullOrEmpty(field.Key))
                    throw new ArgumentException("Fields must have non-empty names");
            }

            if (tags != null)
            {
                foreach (KeyValuePair<string, string> t in tags)
                {
                    if (string.IsNullOrEmpty(t.Key)) throw new ArgumentException("Tags must have non-empty names");
                }
            }

            if (utcTimestamp != null && utcTimestamp.Value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Timestamps must be specified as UTC");

            _measurement = measurement;
            _fields = fields;
            _tags = tags;
            _utcTimestamp = utcTimestamp;
        }

        public void Render(TextWriter textWriter)
        {
            if (textWriter == null) throw new ArgumentNullException(nameof(textWriter));

            textWriter.Write(LineProtocolSyntax.EscapeName(_measurement));

            if (_tags != null)
            {
                foreach (KeyValuePair<string, string> tag in _tags.OrderBy(t => t.Key))
                {
                    if (string.IsNullOrEmpty(tag.Value))
                        continue;

                    textWriter.Write(',');
                    textWriter.Write(LineProtocolSyntax.EscapeName(tag.Key));
                    textWriter.Write('=');
                    textWriter.Write(LineProtocolSyntax.EscapeName(tag.Value));
                }
            }

            char fieldDelim = ' ';
            foreach (KeyValuePair<string, object> field in _fields)
            {
                textWriter.Write(fieldDelim);
                fieldDelim = ',';
                textWriter.Write(LineProtocolSyntax.EscapeName(field.Key));
                textWriter.Write('=');
                textWriter.Write(LineProtocolSyntax.FormatValue(field.Value));
            }

            if (_utcTimestamp != null)
            {
                textWriter.Write(' ');
                textWriter.Write(LineProtocolSyntax.FormatTimestamp(_utcTimestamp.Value));
            }
        }
    }
}