using DadJokeAPI.Models;
using DadJokeConsoleApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


namespace DadJokeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/jokes")]
    public class DadJokeController : ControllerBase
    {
        private readonly JokeService _jokeService;

        private readonly RedisCacheService _redis;

        public DadJokeController(JokeService jokeService, RedisCacheService redis)
        {
            _jokeService = jokeService;
            _redis = redis;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_jokeService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var joke = _jokeService.GetById(id);
            if (joke == null)
                return NotFound();

            return Ok(joke);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string jokeText)
        {
            var newJoke = await _jokeService.Add(jokeText);
            return CreatedAtAction(nameof(GetById), new { id = newJoke.Id }, newJoke);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] string newText)
        {
            var success = await _jokeService.Update(id, newText);
            return success ? Ok("Updated Successfully.") : NotFound("Failed to update.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _jokeService.Delete(id);
            return success ? Ok("Deleted successfully.") : NotFound("Joke not found.");
        }

        //[HttpGet("random")]
        //public IActionResult GetRandom()
        //{
        //    var joke = _jokeService.GetRandom();
        //    return joke == null ? NotFound("No jokes available") : Ok(joke);
        //}

        [HttpGet("random")]
        public async Task<IActionResult> GetRandom()
        {
            var (joke, source) = await _jokeService.GetRandomAsync();

            Response.Headers.Add("X-Data-Source", source);
            return joke != null ? Ok(joke) : NotFound("No jokes found.");
        }
    }
}
