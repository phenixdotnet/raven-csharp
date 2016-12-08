using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreSample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ravenClient = new SharpRaven.RavenClient("http://6f716ba40420499aaadb09a63c665c07:7148b984c13c403fb73f5b589cc1a032@10.37.129.2:9005/2");
            ravenClient.ErrorOnCapture = OnRavenClientError;

            var task = Task.Run(() => MainAsync(args, ravenClient));

            task.Wait();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static async Task MainAsync(string[] args, RavenClient ravenClient)
        {
            var message = new SentryMessage("Starting app");
            var sentryEvent = new SentryEvent(message);
            sentryEvent.Level = ErrorLevel.Debug;
            ravenClient.Capture(sentryEvent);

            try
            {
                throw new InvalidOperationException("Test sentry");
            }
            catch (Exception ex)
            {
                await ravenClient.CaptureAsync(new SentryEvent(ex));
            }
        }

        static void OnRavenClientError(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
