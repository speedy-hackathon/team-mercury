using covidSim.Services;
using Microsoft.AspNetCore.Mvc;

namespace covidSim.Controllers
{
    [Route("api/restart")]
    public class GameRestart : Controller
    {
        [HttpPost]
        public void Index()
        {
            Game.Restart();
        }
    }
}