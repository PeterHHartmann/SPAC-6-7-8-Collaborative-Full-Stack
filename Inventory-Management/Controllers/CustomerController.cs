using Inventory_Management.Managers;
using Inventory_Management.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly CustomerManager _customerManager;

        public CustomerController(CustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerManager.GetAllCustomersAsync();
                if (customers == null || !customers.Any())
                {
                    return NotFound("No customers found");
                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                if (id <= 0) // Check if the ID is valid
                {
                    return BadRequest("Invalid customer ID"); // Return BadRequest if the ID is invalid
                }
                var customer = await _customerManager.GetCustomerByIdAsync(id); // Fetch a single customer by ID
                if (customer == null)
                {
                    return NotFound("No customer found with specified Id"); // Return NotFound if the customer is not found
                }
                return Ok(customer); // Return the customer details
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message); // Return 500 Internal Server Error
            }
        }

        // POST: api/customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customerDto)
        {
            try
            {
                if (customerDto == null)
                {
                    return BadRequest("Customer data is null");
                }
                var createdCustomer = await _customerManager.CreateCustomerAsync(customerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customerDto)
        {
            try
            {
                if (id <= 0 || customerDto == null)
                {
                    return BadRequest("Invalid customer data");
                }
                var updatedCustomer = await _customerManager.UpdateCustomerAsync(id, customerDto);
                if (updatedCustomer == null)
                {
                    return NotFound("No customer found with specified Id");
                }
                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid customer ID");
                }
                var deletedCustomer = await _customerManager.DeleteCustomerAsync(id);
                if (deletedCustomer == null)
                {
                    return NotFound("No customer found with specified Id");
                }
                return Ok(deletedCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
