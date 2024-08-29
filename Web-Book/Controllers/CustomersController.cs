using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Book.Context;
using Web_Book.Models;

namespace Web_Book.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly UserDbContext _context;

        public CustomersController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            return await _context.Customers.ToListAsync();
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return Ok(new { message = "เพิ่มข้อมูลลูกค้าสำเร็จ", customer });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "เพิ่มข้อมูลลูกค้าไม่สำเร็จ", error = ex.Message });
            }
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return BadRequest(new { message = "ID ไม่ตรงกับข้อมูลลูกค้า" });
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "แก้ไขข้อมูลลูกค้าสำเร็จ" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerID == id))
                {
                    return NotFound(new { messge = "ไม่พบข้อมูลลูกค้าที่ต้องการแก้ไข" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "แก้ไขข้อมูลลูกค้าไม่สำเร็จ", error = ex.Message });
            }
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลลูกค้าที่ต้องการลบ" });
            }

            try
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return Ok(new { message = "ลบข้อมูลลูกค้าสำเร็จ" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "ลบข้อมูลลูกค้าไม่สำเร็จ", error = ex.Message });
            }
        }

    }
}
