using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Book.Context;
using Web_Book.Models;

namespace Web_Book.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AddNewsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AddNewsController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetNewsItems()
        {
            return await _context.NewsItems.ToListAsync();
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNewsItem(int id)
        {
            var newsItem = await _context.NewsItems.FindAsync(id);

            if (newsItem == null)
            {
                return NotFound(new { message = "News item not found." });
            }

            return Ok(newsItem);
        }

        // POST: api/News
        [HttpPost]
        public async Task<ActionResult<News>> PostNewsItem(
            [FromForm] string headline,
            [FromForm] string details,
            [FromForm] IFormFile primaryImage,
            [FromForm] List<IFormFile> additionalImages)
        {
            if (additionalImages.Count > 8)
            {
                return BadRequest(new { message = "You can upload a maximum of 8 additional images." });
            }

            var newsItem = new News
            {
                Headline = headline,
                Details = details
            };

            if (primaryImage != null)
            {
                var primaryImagePath = Path.Combine("uploads", primaryImage.FileName);

                try
                {
                    using (var stream = new FileStream(Path.Combine("wwwroot", primaryImagePath), FileMode.Create))
                    {
                        await primaryImage.CopyToAsync(stream);
                    }
                    newsItem.PrimaryImage = primaryImagePath.Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error saving primary image: {ex.Message}" });
                }
            }

            if (additionalImages != null && additionalImages.Count > 0)
            {
                var additionalImagePaths = new List<string>();
                try
                {
                    foreach (var image in additionalImages)
                    {
                        var imagePath = Path.Combine("uploads", image.FileName);
                        using (var stream = new FileStream(Path.Combine("wwwroot", imagePath), FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        additionalImagePaths.Add(imagePath.Replace("\\", "/"));
                    }
                    newsItem.AdditionalImages = additionalImagePaths;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error saving additional images: {ex.Message}" });
                }
            }

            try
            {
                _context.NewsItems.Add(newsItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "News item added successfully!", data = newsItem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error saving news item: {ex.Message}" });
            }
        }

        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNewsItem(
            int id,
            [FromForm] string headline,
            [FromForm] string details,
            [FromForm] IFormFile? newPrimaryImage,
            [FromForm] List<IFormFile>? newAdditionalImages)
        {
            if (newAdditionalImages != null && newAdditionalImages.Count > 8)
            {
                return BadRequest(new { message = "You can upload a maximum of 8 additional images." });
            }

            var existingNewsItem = await _context.NewsItems.FindAsync(id);
            if (existingNewsItem == null)
            {
                return NotFound(new { message = "News item not found." });
            }

            existingNewsItem.Headline = headline;
            existingNewsItem.Details = details;

            if (newPrimaryImage != null)
            {
                var primaryImagePath = Path.Combine("uploads", newPrimaryImage.FileName);
                try
                {
                    using (var stream = new FileStream(Path.Combine("wwwroot", primaryImagePath), FileMode.Create))
                    {
                        await newPrimaryImage.CopyToAsync(stream);
                    }
                    existingNewsItem.PrimaryImage = primaryImagePath.Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error saving primary image: {ex.Message}" });
                }
            }

            if (newAdditionalImages != null && newAdditionalImages.Count > 0)
            {
                var additionalImagePaths = new List<string>();
                try
                {
                    foreach (var image in newAdditionalImages)
                    {
                        var imagePath = Path.Combine("uploads", image.FileName);
                        using (var stream = new FileStream(Path.Combine("wwwroot", imagePath), FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }
                        additionalImagePaths.Add(imagePath.Replace("\\", "/"));
                    }
                    existingNewsItem.AdditionalImages = additionalImagePaths;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Error saving additional images: {ex.Message}" });
                }
            }

            _context.Entry(existingNewsItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "News item updated successfully!", data = existingNewsItem });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Concurrency error occurred while updating the news item." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while updating the news item: {ex.Message}" });
            }
        }

        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsItem(int id)
        {
            var newsItem = await _context.NewsItems.FindAsync(id);
            if (newsItem == null)
            {
                return NotFound(new { message = "News item not found." });
            }

            try
            {
                _context.NewsItems.Remove(newsItem);
                await _context.SaveChangesAsync();
                return Ok(new { message = "News item deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while deleting the news item: {ex.Message}" });
            }
        }

        private bool NewsItemExists(int id)
        {
            return _context.NewsItems.Any(e => e.Id == id);
        }
    }
}
