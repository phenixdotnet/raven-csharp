using System;
using System.Collections.Generic;
using SharpRaven.Data;
using SharpRaven.Logging;

#if !(net40) && !(net35)
using System.Threading.Tasks;
#endif

namespace SharpRaven
{
    /// <inheritdoc />
    public class NoneRavenClient : IRavenClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneRavenClient"/> class
        /// </summary>
        public NoneRavenClient()
        {
            this.Tags = new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public Func<Requester, Requester> BeforeSend
        {
            get;
            set;
        }

        /// <inheritdoc />
        public bool Compression
        {
            get;

            set;
        }

        /// <inheritdoc />
        public Dsn CurrentDsn
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public string Environment
        {
            get;

            set;
        }

        /// <inheritdoc />
        public bool IgnoreBreadcrumbs
        {
            get;

            set;
        }

        /// <inheritdoc />
        public string Logger
        {
            get;

            set;
        }

        /// <inheritdoc />
        public IScrubber LogScrubber
        {
            get;

            set;
        }

        /// <inheritdoc />
        public string Release
        {
            get;

            set;
        }

        /// <inheritdoc />
        public IDictionary<string, string> Tags
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public TimeSpan Timeout
        {
            get;

            set;
        }

        /// <inheritdoc />
        public void AddTrail(Breadcrumb breadcrumb)
        {

        }

        /// <inheritdoc />
        public string Capture(SentryEvent @event)
        {
            return string.Empty;
        }
        
        /// <inheritdoc />
        public string CaptureEvent(Exception e)
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public string CaptureEvent(Exception e, Dictionary<string, string> tags)
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public string CaptureException(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
        {
            return string.Empty;
        }
        
        /// <inheritdoc />
        public string CaptureMessage(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
        {
            return string.Empty;
        }
        
        /// <inheritdoc />
        public void RestartTrails()
        { }

#if !(net40) && !(net35)

        /// <inheritdoc />
        public Task<string> CaptureAsync(SentryEvent @event)
        {
            return Task.FromResult(string.Empty);
        }

        /// <inheritdoc />
        public Task<string> CaptureExceptionAsync(Exception exception, SentryMessage message = null, ErrorLevel level = ErrorLevel.Error, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
        {
            return Task.FromResult(string.Empty);
        }

        /// <inheritdoc />
        public Task<string> CaptureMessageAsync(SentryMessage message, ErrorLevel level = ErrorLevel.Info, IDictionary<string, string> tags = null, string[] fingerprint = null, object extra = null)
        {
            return Task.FromResult(string.Empty);
        }
#endif
    }
}
