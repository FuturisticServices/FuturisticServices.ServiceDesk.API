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

    [Serializable]
    public class MonikerIsRequiredException : Exception
    {
        public MonikerIsRequiredException() : base("Moniker is required.") { }
    }

    [Serializable]
    public class MonikerDoesNotMatchTenantIdException : Exception
    {
        public MonikerDoesNotMatchTenantIdException() : base("Moniker does not match") { }
        public MonikerDoesNotMatchTenantIdException(string moniker) : base(string.Format("Moniker '{0}' does not match.", moniker)) { }
    }
    #endregion Moniker

    #region Subscription
    [Serializable]
    public class SubscriptionIdIsRequiredException : Exception
    {
        public SubscriptionIdIsRequiredException() : base("Subscription ID is required.") { }
    }

    [Serializable]
    public class SubscriptionIsRequiredException : Exception
    {
        public SubscriptionIsRequiredException() : base("Subscription is required.") { }
    }

    [Serializable]
    public class SubscriptionNotFoundException : Exception
    {
        public SubscriptionNotFoundException() : base("Subscription not found.") { }
        public SubscriptionNotFoundException(string id, bool sourceIsSystemDatabase = false) : base(string.Format("Subscription with ID '{0}' not found{1}.", id, sourceIsSystemDatabase ? " in system DB" : string.Empty)) { }
    }
    #endregion Subscription

    #region Billing information
    [Serializable]
    public class BillingInformationNameOnCardIsRequiredException : Exception
    {
        public BillingInformationNameOnCardIsRequiredException() : base("Billing information name on card is required.") { }
    }

    [Serializable]
    public class BillingInformationCardNumberIsRequiredException : Exception
    {
        public BillingInformationCardNumberIsRequiredException() : base("Billing information card number is required.") { }
    }

    [Serializable]
    public class BillingInformationCardNumberIsInvalidException : Exception
    {
        public BillingInformationCardNumberIsInvalidException() : base("Billing information card number is invalid.") { }
    }

    [Serializable]
    public class BillingInformationCardSecurityNumberIsRequiredException : Exception
    {
        public BillingInformationCardSecurityNumberIsRequiredException() : base("Billing information card security value (CSV) is required.") { }
    }

    [Serializable]
    public class BillingInformationCardSecurityNumberIsInvalidException : Exception
    {
        public BillingInformationCardSecurityNumberIsInvalidException() : base("Billing information card security value (CSV) is invalid.") { }
    }

    [Serializable]
    public class BillingInformationExpirationMonthIsRequiredException : Exception
    {
        public BillingInformationExpirationMonthIsRequiredException() : base("Billing information expiration month is required.") { }
    }

    [Serializable]
    public class BillingInformationExpirationMonthIsInvalidException : Exception
    {
        public BillingInformationExpirationMonthIsInvalidException() : base("Billing information expiration month invalid is required.") { }
    }

    [Serializable]
    public class BillingInformationExpirationYearIsRequiredException : Exception
    {
        public BillingInformationExpirationYearIsRequiredException() : base("Billing information expiration year is required.") { }
    }

    [Serializable]
    public class BillingInformationExpirationYearIsInvalidException : Exception
    {
        public BillingInformationExpirationYearIsInvalidException() : base("Billing information expiration year invalid is required.") { }
    }
    #endregion Billing information

    #region Company
    [Serializable]
    public class CompanyNameIsRequiredException : Exception
    {
        public CompanyNameIsRequiredException() : base("Company name is required.") { }
    }
    #endregion Company

    #region Address
    [Serializable]
    public class AddressTypeIdIsRequiredException : Exception
    {
        public AddressTypeIdIsRequiredException() : base("Address type ID is required.") { }
    }

    [Serializable]
    public class AddressLine1IsRequiredException : Exception
    {
        public AddressLine1IsRequiredException() : base("Address line 1 is required.") { }
    }

    [Serializable]
    public class AddressCityIsRequiredException : Exception
    {
        public AddressCityIsRequiredException() : base("Address city is required.") { }
    }

    [Serializable]
    public class AddressStateIdIsRequiredException : Exception
    {
        public AddressStateIdIsRequiredException() : base("Address state ID is required.") { }
    }

    [Serializable]
    public class AddressPostalCodeIsRequiredException : Exception
    {
        public AddressPostalCodeIsRequiredException() : base("Address postal code is required.") { }
    }

    [Serializable]
    public class AddressPostalCodeIsInvalidException : Exception
    {
        public AddressPostalCodeIsInvalidException() : base("Address postal code is invalid.") { }
    }

    [Serializable]
    public class AddressCountryIdIsRequiredException : Exception
    {
        public AddressCountryIdIsRequiredException() : base("Address country ID is required.") { }
    }
    #endregion Address

    #region Phone number
    [Serializable]
    public class PhoneNumberIsRequiredException : Exception
    {
        public PhoneNumberIsRequiredException() : base("Phone number is required.") { }
    }
    #endregion Address

    #region Email address
    [Serializable]
    public class EmailAddressIsRequiredException : Exception
    {
        public EmailAddressIsRequiredException() : base("Email address is required.") { }
    }
    #endregion Email address

    #region Website
    [Serializable]
    public class WebsiteUrlIsRequiredException : Exception
    {
        public WebsiteUrlIsRequiredException() : base("Website URL is required.") { }
    }
    #endregion Website

    #region Point of contact
    [Serializable]
    public class PointOfContactFirstNameIsRequiredException : Exception
    {
        public PointOfContactFirstNameIsRequiredException() : base("Point of contact first name is required.") { }
    }

    [Serializable]
    public class PointOfContactLastNameIsRequiredException : Exception
    {
        public PointOfContactLastNameIsRequiredException() : base("Point of contact last name is required.") { }
    }

    [Serializable]
    public class PointOfContactTitleIsRequiredException : Exception
    {
        public PointOfContactTitleIsRequiredException() : base("Point of contact title is required.") { }
    }
    #endregion Point of contact
}
