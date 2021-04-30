using covidSim.Models;
using covidSim.Services;
using Microsoft.AspNetCore.Mvc;

namespace covidSim.Controllers
{
    [Route("api/action")]
    public class ActionController : Controller
    {
        [HttpGet]
        public void Index()
        {
            Game.Restart();
        }
        public IActionResult Action([FromBody] UserActionDto userAction)
        {
            var game = Game.Instance;
            var person = game.People.Find(p => p.Id == userAction.PersonClicked);
            person.GoHome();
            return NoContent();
        }
    }
}