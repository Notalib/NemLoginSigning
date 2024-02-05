using System;

using Microsoft.AspNetCore.Http;

namespace NemLoginSigningWebApp.Utils
{
    /// <summary>
    /// Utility class to get the Correlation Id given by header in the request - or a new one.
    /// </summary>
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        public const string HeaderName = "X-Correlation-Id";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<Guid> _fallBackGuid = new Lazy<Guid>(() => Guid.NewGuid());

        public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get the correlation ID set by the client in the X-Correlation-Id HTTP header.
        /// If it is not set in the header, a new GUID is generated and returned.
        /// </summary>
        /// <remarks>
        /// If a new GUID is generated, it is consistant within each instance of this class.
        /// </remarks>
        /// <example>
        /// // In Startup.cs:
        /// services.AddCorrelationIdAccessor();
        ///
        /// // In other classes:
        /// public MyClass(ICorrelationIdAccessor correlationIdAccessor)
        /// {
        ///    _correlationId = correlationIdAccessor.Id;
        /// }
        /// </example>
        public Guid Id
        {
            get
            {
                Microsoft.Extensions.Primitives.StringValues cid = _httpContextAccessor.HttpContext.Request.Headers[HeaderName];

                if (Guid.TryParse(cid.ToString(), out Guid correlationId))
                {
                    return correlationId;
                }

                // If header was not set on request or parsing failed, look for it in response;
                // Some middleware, e.g. Serilog.Enrichers.CorrelationIdHeaderEnricher sets it there.
                cid = _httpContextAccessor.HttpContext.Response.Headers[HeaderName];

                if (Guid.TryParse(cid.ToString(), out Guid responseCorrelationId))
                {
                    return responseCorrelationId;
                }

                // That failed, too. Make one up.
                return _fallBackGuid.Value;
            }
        }
    }
}
