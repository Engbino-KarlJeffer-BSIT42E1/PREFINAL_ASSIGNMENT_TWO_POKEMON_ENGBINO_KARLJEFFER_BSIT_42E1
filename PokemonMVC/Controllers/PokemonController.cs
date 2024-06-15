using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PokemonMVC.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokemonMVC.Controllers
{
    public class PokemonController : Controller
    {
        private readonly HttpClient _httpClient;

        public PokemonController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("https://pokeapi.co/api/v2/");
        }

        public IActionResult Indexx()
        {
            return View();
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var response = await _httpClient.GetAsync($"pokemon?offset={(page - 1) * 20}&limit=20");
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var results = json["results"].ToObject<List<Pokemon>>();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(json["count"].ToObject<int>() / 20.0);

            return View(results);
        }

        public async Task<IActionResult> Details(string name)
        {
            var response = await _httpClient.GetAsync($"pokemon/{name}");
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var pokemon = new Pokemon
            {
                Name = json["name"].ToString(),
                Moves = json["moves"].ToObject<List<JObject>>().Select(m => m["move"]["name"].ToString()).ToList(),
                Abilities = json["abilities"].ToObject<List<JObject>>().Select(a => a["ability"]["name"].ToString()).ToList()
            };

            return View(pokemon);

        }
    }
}
