using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuturisticServices.ServiceDesk.API.Common
{
    [Serializable]
    public class TenantDoesNotExistException : Exception
    {
        public TenantDoesNotExistException() { }

        public TenantDoesNotExistException(string moniker) : base(string.Format("Tenant '{0}' does NOT exist.", moniker)) { }

    }

    [Serializable]
    public class MonikerAlreadyExistsException : Exception
    {
        public MonikerAlreadyExistsException() { }

        public MonikerAlreadyExistsException(string moniker) : base(string.Format("Moniker '{0}' already exists.", moniker)) { }

    }

    [Serializable]
    public class MonikerDoesNotExistException : Exception
    {
        public MonikerDoesNotExistException() {}

        public MonikerDoesNotExistException(string moniker) : base(string.Format("Moniker '{0}' does NOT exist.", moniker)) { }

    }
}
