using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    /// <summary>
    /// Tangled.Services CustomersContoller class.
    /// </summary>
    [ApiController, ApiVersion("1.0")]
    [Route("api/customers")]
    public class CustomerController : BaseController
    {
        #region Properties
        private readonly ICustomerService _customerService;
        private readonly ISystemService _systemService;
        private readonly IConfiguration _configuration;
        #endregion Properties

        #region Constructors
        /// <summary>
        /// CustomersController constructors.
        /// </summary>
        /// <param name="adminService"></param>
        /// <param name="configuration"></param>
        public CustomerController(ICustomerService customerService, ISystemService systemService, IConfiguration configuration)
        {
            _customerService = customerService;
            _systemService = systemService;
            _configuration = configuration;
        }
        #endregion Constructors

        #region APIs
        /// <summary>
        /// Retrieves a customer by the administrative moniker.
        /// </summary>
        /// <param name="adminMoniker">Customer admin identifier</param>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet("{moniker}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string adminMoniker)
        {
            try
            {
                adminMoniker = adminMoniker.ToUpper().Trim();
                var tenant = await _customerService.Get(adminMoniker);

                CustomerEntity customerEntity = await _customerService.Get(adminMoniker);
                CustomerModel model = new CustomerModel(customerEntity);

                responseModels.Add("Customers", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Customer '{0}' found.", adminMoniker), responseModels);
                return Ok(new { response });
            }
            catch (CustomerNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, string.Format("Customer '{0}' not found in the system database.", adminMoniker), exception, null);
                return Ok(new { response });
            }
            catch (UnauthorizedAccessException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Access denied", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Create a Customer object.
        /// </summary>
        /// <param name="model">CustomerModel object.</param>
        /// <returns></returns>
        [HttpPost("{moniker}"), MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(string moniker, [FromBody] CustomerModel model)
        {
            try
            {
                CustomerModel customerModel = await _customerService.Create(model);

                responseModels.Add("Customer", customerModel);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Customer '{0}' created in the system database successfully.", model.Name), null, responseModels);
                return Ok(new { response });
            }
            catch (SystemDatabaseNotCreatedException exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "Error creating customer database.", exception, null);
                return Ok(new { response });
            }
            catch (CustomerAlreadyExistsException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, string.Format("Customer with name '{0}' already exists in the system database.", model.Name), null);
                return Ok(new { response });
            }
            
            catch (MonikerAlreadyExistsException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, string.Format("Customer with admin moniker '{0}' already exists in the system database.", model.AdminMoniker), null);
                return Ok(new { response });
            }
            catch (UnauthorizedAccessException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Access denied", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "Error creating customer in the system database.", exception, null);
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Updates a SystemTenant item in the system database.
        /// </summary>
        /// <param name="model">SystemTenantModel object</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] CustomerModel model)
        {
            try
            {
                model = await _customerService.Update(model);

                responseModels.Add("Customers", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Customer '{0}' updated in system DB successfully.", model.Name), responseModels);
                return Ok(new { response });
            }
            catch (UnauthorizedAccessException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Access denied", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// "Deletes" (mark as disabled) a SystemTenant item in the system database.
        /// </summary>
        /// <param name="model">Model depicting the tenant to update.</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromBody] IdModel model)
        {
            try
            {
                CustomerModel customerModel = await _customerService.Delete(model);
                response = new ApiResponse(HttpStatusCode.OK, "Customer deleted in system DB successfully.", null, null);
                return Ok(new { response });
            }
            catch (UnauthorizedAccessException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Access denied", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Customer with ID '{0}' deleted from the system database was unsuccessful.", model.Id), exception, null);
                return BadRequest(new { response });
            }
        }
        #endregion APIs
    }
}