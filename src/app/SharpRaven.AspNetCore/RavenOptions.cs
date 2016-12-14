using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpRaven.AspNetCore
{
    public class RavenOptions : IOptions<RavenOptions>
    {
        public RavenOptions()
        {
            this.Compression = false;
            this.Timeout = new TimeSpan(0, 0, 5);
        }

        /// <summary>
        /// Gets or sets the Dsn value
        /// </summary>
        public string Dsn
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the compression is enabled or not
        /// </summary>
        public bool Compression
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if raven should ignore the breadcrumbs or not
        /// </summary>
        public bool IgnoreBreadcrumbs
        { get; set; }

        /// <summary>
        /// Gets or sets the logger name
        /// </summary>
        public string Logger
        { get; set; }

        /// <summary>
        /// Gets or sets the timeout limit
        /// </summary>
        public TimeSpan Timeout
        { get; set; }

        /// <summary>
        /// Gets or sets the release version or name
        /// </summary>
        public string Release
        { get; set; }

        /// <summary>
        /// Gets or sets the environment name
        /// </summary>
        public string Environment
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RavenOptions Value
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
