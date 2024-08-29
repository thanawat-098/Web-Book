using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Book.Context;
using Web_Book.Models;

namespace Web_Book.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RoomsController(UserDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomImages)
                .ToListAsync();

            return Ok(rooms);
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom([FromForm] Room room, IFormFile? imageFile)
        {
            try
            {
                // ตรวจสอบความถูกต้องของข้อมูลห้องพัก
                if (room == null)
                {
                    return BadRequest(new { message = "จำเป็นต้องมีข้อมูลห้อง" });
                }

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                if (imageFile != null)
                {
                    if (!IsValidImageFile(imageFile))
                    {
                        return BadRequest(new { message = "รูปแบบไฟล์ภาพไม่ถูกต้อง" });
                    }

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "ImagesRoom");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the image file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var roomImage = new RoomImage
                    {
                        RoomID = room.RoomID,
                        ImagePath = "/ImagesRoom/" + uniqueFileName
                    };

                    _context.RoomImages.Add(roomImage);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "เพิ่มห้องเรียบร้อยแล้ว!" });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                return BadRequest(new { message = $"Failed to add room. Error: {ex.Message}" });
            }
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }


        // PUT: api/rooms/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, [FromForm] Room room, IFormFile? imageFile)
        {
            if (id != room.RoomID)
            {
                return BadRequest(new { message = "รหัสห้องไม่ตรงกัน" });
            }

            var existingRoom = await _context.Rooms
                .Include(r => r.RoomImages)
                .FirstOrDefaultAsync(r => r.RoomID == id);

            if (existingRoom == null)
            {
                return NotFound(new { message = "ไม่พบห้อง" });
            }

            // อัปเดตข้อมูลห้อง
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomType = room.RoomType;
            existingRoom.Price = room.Price;
            existingRoom.Status = room.Status;

            try
            {
                if (imageFile != null)
                {
                    if (!IsValidImageFile(imageFile))
                    {
                        return BadRequest(new { message = "รูปแบบไฟล์ภาพไม่ถูกต้อง" });
                    }

                    var existingImage = await _context.RoomImages.FirstOrDefaultAsync(ri => ri.RoomID == room.RoomID);
                    if (existingImage != null)
                    {
                        // Remove old image
                        var oldImagePath = Path.Combine(_env.WebRootPath, existingImage.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(new { message = $"Failed to delete old image. Error: {ex.Message}" });
                            }
                        }

                        _context.RoomImages.Remove(existingImage);
                    }

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "ImagesRoom");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                    try
                    {
                        using (var stream = new FileStream(newFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"Failed to save new image. Error: {ex.Message}" });
                    }

                    var newRoomImage = new RoomImage
                    {
                        RoomID = room.RoomID,
                        ImagePath = "/ImagesRoom/" + uniqueFileName
                    };

                    _context.RoomImages.Add(newRoomImage);
                }
                else
                {
                    // If no new image file is uploaded, retain the old image.
                    // No need to add or remove any image entries.
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "อัปเดตห้องเรียบร้อยแล้ว" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound(new { message = "ไม่พบห้อง" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"เกิดข้อผิดพลาดในการอัปเดตห้อง. ข้อความผิดพลาด: {ex.Message}" });
            }
        }


        // DELETE: api/rooms/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound(new {message = "ไม่พบห้องที่ต้องการลบ" });
            }

            var images = await _context.RoomImages.Where(ri => ri.RoomID == id).ToListAsync();
            foreach (var image in images)
            {
                var imagePath = Path.Combine(_env.WebRootPath, image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = $"ลบรูปภาพไม่สำเร็จ. ข้อความผิดพลาด: {ex.Message}" });
                    }
                }
                _context.RoomImages.Remove(image);
            }

            _context.Rooms.Remove(room);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "ลบห้องเรียบร้อยแล้ว" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"ลบห้องไม่สำเร็จ. ข้อความผิดพลาด: {ex.Message}" });
            }
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomID == id);
        }
    }
}

