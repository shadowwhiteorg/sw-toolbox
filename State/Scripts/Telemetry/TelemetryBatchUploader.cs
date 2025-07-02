using System;
using System.Collections.Generic;
using _t.Shared.Batching;
using UnityEngine;

namespace _t.Unity.Telemetry
{
    /// <summary>
    /// MonoBehaviour wrapper around BatchDispatcher, grouping telemetry by event.Type.
    /// </summary>
    public class TelemetryBatchUploader : MonoBehaviour
    {
        [Tooltip("Number of events per type to batch before immediate upload")]
        public int BatchSize = 10;
        
        [Tooltip("Time interval (seconds) after which a pending batch is dispatched")]
        public float DispatchInterval = 5f;
        
        private BatchDispatcher<string, TelemetryEvent> _dispatcher;
        
        private void Awake()
        {
            _dispatcher = new BatchDispatcher<string, TelemetryEvent>(
                batchSize: BatchSize,
                dispatchInterval: TimeSpan.FromSeconds(DispatchInterval),
                onDispatch: UploadBatch
            );
        }
        
        private void Update()
        {
            _dispatcher.Tick();
        }
        
        /// <summary>
        /// Call to log a telemetry event under its Type group.
        /// </summary>
        public void Log(string type, string payload)
        {
            var evt = new TelemetryEvent(type, payload);
            _dispatcher.Enqueue(type, evt);
        }
        
        /// <summary>
        /// Immediately flush all pending telemetry batches.
        /// </summary>
        public void FlushAll()
        {
            _dispatcher.FlushAll();
        }
        
        private void OnApplicationQuit()
        {
            FlushAll();
        }
        
        private void UploadBatch(string type, List<TelemetryEvent> batch)
        {
            Debug.Log($"[Telemetry] Uploading {batch.Count} events of type '{type}'");
            foreach (var evt in batch)
            {
                Debug.Log($"→ [{evt.Timestamp:F2}] {evt.Type}: {evt.Payload}");
            }
        
            // TODO: send `batch` to your server endpoint
        }
    }
}