namespace _t.Unity.Telemetry
{
    public class TelemetryEvent
    {
        public string Type;
        public string Payload;
        public float Timestamp;

        public TelemetryEvent(string type, string payload)
        {
            Type = type;
            Payload = payload;
            Timestamp = UnityEngine.Time.time;
        }
    }
}