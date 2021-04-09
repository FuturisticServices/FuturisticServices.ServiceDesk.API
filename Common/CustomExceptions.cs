using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Common
{
    #region LookupItems
    [Serializable]
    public class LookupItemsNotFoundException : Exception
    {
        public LookupItemsNotFoundException() { }

        public LookupItemsNotFoundException(bool sourceIsSystemDatabase = false) : base(string.Format("Lookup items not found{0}.", sourceIsSystemDatabase ? "in system DB" : string.Empty)) { }
    }
    #endregion LookupItems

    #region SystemTenant
    [Serializable]
    public class SystemTenantDoesNotExistException : Exception
    {
        public SystemTenantDoesNotExistException() { }

        public SystemTenantDoesNotExistException(string moniker) : base(string.Format("Tenant '{0}' does not exist in system DB.", moniker)) { }

    }

    [Serializable]
    public class SystemTenantAlreadyExistsException : Exception
    {
        public SystemTenantAlreadyExistsException() { }

        public SystemTenantAlreadyExistsException(string moniker) : base(string.Format("Tenant '{0}' already exists in system DB.", moniker)) { }

    }
    #endregion SystemTenants

    #region SystemDatabase
    [Serializable]
    public class SystemDatabaseNotCreatedException : Exception
    {
        public SystemDatabaseNotCreatedException() { }

        public SystemDatabaseNotCreatedException(string moniker) : base("System database failed to create.") { }
    }
    #endregion SystemDatabase

    #region Tenant
    [Serializable]
    public class TenantSetupNoContainersFoundToCreateException : Exception
    {
        public TenantSetupNoContainersFoundToCreateException() : base(string.Format("No 'create' containers found in config file '{0}'.", "tenantSetup.json")) { }
    }

    [Serializable]
    public class TenantSetupCreateContainerFailedException : Exception
    {
        public TenantSetupCreateContainerFailedException(string containerName) : base(string.Format("Tenant container '{0}' failed to create.", containerName)) { }
    }

    [Serializable]
    public class TenantSetupDatabaseCouldNotBeCreatedException : Exception
    {
        public TenantSetupDatabaseCouldNotBeCreatedException() { }

        public TenantSetupDatabaseCouldNotBeCreatedException(string moniker) : base(string.Format("Database for tenant '{0}' could not be created.", moniker)) { }
    }

    [Serializable]
    public class TenantDoesNotExistException : Exception
    {
        public TenantDoesNotExistException() { }

        public TenantDoesNotExistException(string moniker) : base(string.Format("Tenant '{0}' does NOT exist in system DB.", moniker)) { }
    }
    #endregion Tenant

    #region Moniker
    [Serializable]
    public class MonikerAlreadyExistsException : Exception
    {
        public MonikerAlreadyExistsException() { }

        public MonikerAlreadyExistsException(string moniker) : base(string.Format("Moniker '{0}' already exists.", moniker)) { }
    }

    [Serializable]
    public class MonikerDoesNotExistException : Exception
    {
        public MonikerDoesNotExistException() { }

        public MonikerDoesNotExistException(string moniker) : base(string.Format("Moniker '{0}' does NOT exist.", moniker)) { }
    }
    #endregion Moniker

    #region Subscription
    [Serializable]
    public class SubscriptionNotFoundException : Exception
    {
        public SubscriptionNotFoundException() : base("Subscription not found.") { }
        public SubscriptionNotFoundException(string id, bool sourceIsSystemDatabase = false) : base(string.Format("Subscription with ID '{0}' not found{1}.", id, sourceIsSystemDatabase ? " in system DB" : string.Empty)) { }
    }
    #endregion Subscription
}
