using System;
using System.Collections.Generic;
using UnityEngine;
using _t.Shared.Batching;

namespace _t.Unity.Telemetry
{
    public class TelemetryBatchUploader : MonoBehaviour
    {
        private IBatchDispatcher<TelemetryEvent> _dispatcher;

        private void Awake()
        {
            _dispatcher = new BatchDispatcher<TelemetryEvent>(
                batchSize: 10,
                dispatchInterval: TimeSpan.FromSeconds(5),
                onDispatch: UploadTelemetryBatch
            );
        }

        private void Update()
        {
            _dispatcher.Tick();
        }

        public void LogEvent(string type, string payload)
        {
            var evt = new TelemetryEvent(type, payload);
            _dispatcher.Enqueue(evt);
        }

        private void UploadTelemetryBatch(List<TelemetryEvent> batch)
        {
            Debug.Log($"Uploading {batch.Count} telemetry events");

            foreach (var evt in batch)
                Debug.Log($"→ [{evt.Timestamp}] {evt.Type}: {evt.Payload}");

            // TODO: Send batch to server or file
        }
    }
}