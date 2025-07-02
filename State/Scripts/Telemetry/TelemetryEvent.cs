namespace _t.Unity.Telemetry
{
    /// <summary>
    /// Simple telemetry payload object.
    /// </summary>
    public class TelemetryEvent
    {
        public string Type { get; }
        public string Payload { get; }
        public float Timestamp { get; }

        public TelemetryEvent(string type, string payload)
        {
            Type = type;
            Payload = payload;
            Timestamp = UnityEngine.Time.time;
        }
    }
}