using System;

namespace NemLoginSigningWebApp.Utils
{
    public interface ICorrelationIdAccessor
    {
        Guid Id { get; }
    }
}
