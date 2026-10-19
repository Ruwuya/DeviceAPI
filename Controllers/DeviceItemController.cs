using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeviceAPI.Models;

namespace DeviceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceItemController : ControllerBase
    {
        private readonly DeviceContext _context;

        public DeviceItemController(DeviceContext context)
        {
            _context = context;
        }

        // GET: api/DeviceItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceItemDTO>>> GetDeviceItems()
        {
            return await _context.DeviceItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // GET: api/DeviceItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceItemDTO>> GetDeviceItem(long id)
        {
            var deviceItem = await _context.DeviceItems.FindAsync(id);

            if (deviceItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(deviceItem);
        }

        // PUT: api/DeviceItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceItem(long id, DeviceItemDTO deviceDTO)
        {
            if (id != deviceDTO.Id)
            {
                return BadRequest();
            }

            var deviceItem = await _context.DeviceItems.FindAsync(id);
            if (deviceItem == null)
            {
                return NotFound();
            }

            deviceItem.Name = deviceDTO.Name;
            deviceItem.Type = deviceDTO.Type;
            deviceItem.Status = deviceDTO.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DeviceItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/DeviceItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeviceItemDTO>> PostDeviceItem(DeviceItemDTO deviceDTO)
        {
            var deviceItem = new DeviceItem
            {
                Name = deviceDTO.Name,
                Type = deviceDTO.Type,
                Status = deviceDTO.Status
            };

            _context.DeviceItems.Add(deviceItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDeviceItem),
                new { id = deviceItem.Id },
                ItemToDTO(deviceItem));
        }

        // DELETE: api/DeviceItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceItem(long id)
        {
            var deviceItem = await _context.DeviceItems.FindAsync(id);
            if (deviceItem == null)
            {
                return NotFound();
            }

            _context.DeviceItems.Remove(deviceItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceItemExists(long id)
        {
            return _context.DeviceItems.Any(e => e.Id == id);
        }

        private static DeviceItemDTO ItemToDTO(DeviceItem deviceItem) =>
            new DeviceItemDTO
            {
                Id = deviceItem.Id,
                Name = deviceItem.Name,
                Type = deviceItem.Type,
                Status = deviceItem.Status
            };
    }
}
