using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpRaven.Data;

namespace SharpRaven.AspNetCore
{
    public class RavenExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly IRavenClient ravenClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenExceptionFilter"/> class
        /// </summary>
        /// <param name="ravenClient">The raven client instance to be used</param>
        public RavenExceptionFilter(IRavenClient ravenClient)
        {
            if (ravenClient == null)
                throw new ArgumentNullException(nameof(ravenClient));

            this.ravenClient = ravenClient;
        }

        /// <summary>
        /// Handle the exception
        /// </summary>
        /// <param name="context">The current context</param>        
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await HandleException(context).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle the exception
        /// </summary>
        /// <param name="context">The current context</param>
        public void OnException(ExceptionContext context)
        {
            var task = Task.Run(() => HandleException(context));
            task.Wait();
        }

        private async Task HandleException(ExceptionContext context)
        {
            var sentryEvent = new SentryEvent(context.Exception);
            await this.ravenClient.CaptureAsync(sentryEvent);
        }
    }
}
