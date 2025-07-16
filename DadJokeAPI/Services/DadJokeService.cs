using System.Text.Json;
using DadJokeAPI.Models;
using DadJokeConsoleApi.Data;

namespace DadJokeConsoleApi.Services
{
    public class JokeService
    {

        private readonly RedisCacheService _redis;

        private readonly DadJokeDbContext _context;

        public JokeService(DadJokeDbContext context, RedisCacheService redis)
        {
            _context = context;
            _redis = redis;
        }

        public List<DadJoke> GetAll() => _context.Jokes.ToList();

        public DadJoke GetById(int id) => _context.Jokes.Find(id);

        public async Task<DadJoke> Add(string jokeText)
        {
            var joke = new DadJoke { Joke = jokeText };
            _context.Jokes.Add(joke);
            _context.SaveChanges();
            await _redis.RemoveKeyAsync("all_jokes");
            return joke;
        }

        public async Task<bool> Delete(int id)
        {
            var joke = GetById(id);
            if (joke == null) return false;
            _context.Jokes.Remove(joke);
            _context.SaveChanges();
            await _redis.RemoveKeyAsync("all_jokes");
            return true;
        }

        public async Task<bool> Update(int id, string newText)
        {
            var joke = GetById(id);
            if (joke == null) return false;
            joke.Joke = newText;
            _context.SaveChanges();
            await _redis.RemoveKeyAsync("all_jokes");
            return true;
        }

        //public DadJoke? GetRandom()
        //{
        //    var count = _context.Jokes.Count();
        //    if (count == 0) return null;

        //    var index = new Random().Next(0, count);
        //    return _context.Jokes.Skip(index).FirstOrDefault();
        //}

        public async Task<(DadJoke? joke, string source)> GetRandomAsync()
        {
            string cacheKey = "all_jokes";

            var cached = await _redis.GetValueAsync(cacheKey);
            List<DadJoke>? jokes;
            string source;

            if (cached != null)
            {
                jokes = JsonSerializer.Deserialize<List<DadJoke>>(cached);
                source = "Redis";
            }
            else
            {
                jokes = _context.Jokes.ToList();
                if (jokes == null || jokes.Count == 0) return (null, "SQL Server");

                var jokesJson = JsonSerializer.Serialize(jokes);
                await _redis.SetValueAsync(cacheKey, jokesJson, TimeSpan.FromMinutes(5));
                source = "SQL Server";
            }

            var randomIndex = new Random().Next(0, jokes.Count);
            return (jokes[randomIndex], source);
        }


    }
}
