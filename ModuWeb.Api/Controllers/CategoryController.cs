using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        // In-memory category list for demo; replace with DB or config in production
        private static readonly List<string> Categories = new List<string>
        {
            "Shoes", "Clothing", "Electronics", "Books", "Accessories"
        };

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetCategories()
        {
            return Categories;
        }

        [HttpPost]
        public ActionResult AddCategory([FromBody] string category)
        {
            if (string.IsNullOrWhiteSpace(category) || Categories.Contains(category))
                return BadRequest("Invalid or duplicate category.");
            Categories.Add(category);
            return Ok();
        }
    }
}
