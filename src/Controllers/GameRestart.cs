using covidSim.Services;
using Microsoft.AspNetCore.Mvc;

namespace covidSim.Controllers
{
    [Route("api/restart")]
    public class GameRestart : Controller
    {
        // GET
        [HttpGet]
        public bool Index()
        {
            Game.Restart();
            return true;
        }
    }
}