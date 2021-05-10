using System;

namespace TangledServices.ServicePortal.API.Common
{
    #region LookupItems
    [Serializable]
    public class SystemLookupItemsNotFoundException : Exception
    {
        public SystemLookupItemsNotFoundException() : base("No LookupItems found in system database.") { }
    }

    public class SystemLookupItemNotFoundException : Exception
    {
        public SystemLookupItemNotFoundException() { }

        public SystemLookupItemNotFoundException(Guid id) : base(string.Format("LookupItem with ID '{0}' not found in system database.", id.ToString())) { }

        public SystemLookupItemNotFoundException(string name) : base(string.Format("LookupItem '{0}' not found in system database.", name)) { }
    }

    public class SystemLookupItemAlreadyExistsException : Exception
    {
        public SystemLookupItemAlreadyExistsException() { }

        public SystemLookupItemAlreadyExistsException(string name) : base(string.Format("LookupItem '{0}' already exists in system database.", name)) { }
    }
    #endregion LookupItems

    #region Customers
    [Serializable]
    public class CustomerNotActiveException : Exception
    {
        public CustomerNotActiveException() { }

        public CustomerNotActiveException(string name) : base(string.Format("Customer '{0}' in the system database is NOT active.", name)) { }

    }

    [Serializable]
    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException() { }

        public CustomerNotFoundException(string name) : base(string.Format("Customer '{0}' not found in the system database.", name)) { }

    }

    [Serializable]
    public class CustomerAlreadyExistsException : Exception
    {
        public CustomerAlreadyExistsException() { }

        public CustomerAlreadyExistsException(string name) : base(string.Format("Customer '{0}' already exists in the system .", name)) { }

    }

    [Serializable]
    public class EntityNameIsRequiredException : Exception
    {
        public EntityNameIsRequiredException() { }
        public EntityNameIsRequiredException(string name) : base("Entity name is required.") { }
    }
    #endregion Customers

    #region System
    [Serializable]
    public class SystemDatabaseNotCreatedException : Exception
    {
        public SystemDatabaseNotCreatedException() { }

        public SystemDatabaseNotCreatedException(string moniker) : base("System database failed to create.") { }
    }
    #endregion System

    #region Admin
    [Serializable]
    public class AdminDatabaseNotCreatedException : Exception
    {
        public AdminDatabaseNotCreatedException() { }

        public AdminDatabaseNotCreatedException(string databaseName) : base(string.Format("Admin database '{0}' failed to create.", databaseName)) { }
    }

    [Serializable]
    public class AdminContainerNotCreatedException : Exception
    {
        public AdminContainerNotCreatedException() { }

        public AdminContainerNotCreatedException(string containerName) : base(string.Format("Admin container '{0}' failed to create.", containerName)) { }
    }
    #endregion Admin

    #region Tenant
    [Serializable]
    public class TenantRegistrationNoContainersFoundToCreateException : Exception
    {
        public TenantRegistrationNoContainersFoundToCreateException() : base(string.Format("No 'create' containers found in config file '{0}'.", "tenantRegistration.json")) { }
    }

    [Serializable]
    public class TenantRegistrationCreateContainerFailedException : Exception
    {
        public TenantRegistrationCreateContainerFailedException(string containerName) : base(string.Format("Tenant container '{0}' failed to create.", containerName)) { }
    }

    [Serializable]
    public class TenantRegistrationDatabaseCouldNotBeCreatedException : Exception
    {
        public TenantRegistrationDatabaseCouldNotBeCreatedException() { }

        public TenantRegistrationDatabaseCouldNotBeCreatedException(string moniker) : base(string.Format("Database for tenant '{0}' could not be created.", moniker)) { }
    }

    [Serializable]
    public class TenantNotFoundException : Exception
    {
        public TenantNotFoundException() { }

        public TenantNotFoundException(string moniker) : base(string.Format("Tenant '{0}' NOT found in system DB.", moniker)) { }
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
    public class MonikerNotFoundException : Exception
    {
        public MonikerNotFoundException() { }

        public MonikerNotFoundException(string moniker) : base(string.Format("Moniker '{0}' NOT found.", moniker)) { }
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

    #region Fields
    [Serializable]
    public class NameIsRequiredException : Exception
    {
        public NameIsRequiredException() : base("Name is required.") { }
    }
    #endregion Fields

    #region Departments
    [Serializable]
    public class DepartmentsNotFoundException : Exception
    {
        public DepartmentsNotFoundException() : base("No departments found.") { }
    }

    [Serializable]
    public class DepartmentNotFoundException : Exception
    {
        public DepartmentNotFoundException() : base("Department NOT found.") { }
        public DepartmentNotFoundException(string name) : base(string.Format("Department '{0}' NOT found.", name)) { }
    }
    #endregion Departments

    #region Login
    [Serializable]
    public class LoginFailedException : Exception
    {
        public LoginFailedException() : base("Login failed.") { }
    }

    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found.") { }
    }
    #endregion Login

    #region User
    [Serializable]
    public class PasswordsDoNotMatchException : Exception
    {
        public PasswordsDoNotMatchException() : base("Passwords do not match.") { }
    }

    [Serializable]
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException(string username) : base(string.Format("Username '{0}' already exists.", username)) { }
    }
    #endregion User

    #region GUID
    [Serializable]
    public class GuidNotValidException : Exception
    {
        public GuidNotValidException() : base("GUID not valid.") { }
    }
    #endregion GUID
}
