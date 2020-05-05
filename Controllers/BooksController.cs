using Fisher.Bookstore.Models;
using Fisher.Bookstore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fisher.Bookstore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private IBooksRepository booksRepository;

        public BooksController(IBooksRepository repository)
        {
            booksRepository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(booksRepository.GetBooks());
        }  

    }
      
}