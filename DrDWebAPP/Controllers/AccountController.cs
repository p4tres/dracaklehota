using DrDWebAPP.Data;
using DrDWebAPP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DrDWebAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly DrDContext _drdContext;
        public AccountController(DrDContext drdContext)
        {
            _drdContext = drdContext;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> LogIn(LoggedInUser logUser)
        {
            var profile = await _drdContext.Users.FirstOrDefaultAsync(context => context.Nickname == logUser.UserName && context.Password == logUser.Password);
            if (profile != null)
            {
                Response.Cookies.Append("UserId", ((int)profile.UserId).ToString(), new CookieOptions
                {
                    //Expires = DateTimeOffset.UtcNow.AddDays(1),
                    HttpOnly = true,
                    IsEssential = false
                });
                Response.Cookies.Append("UserName", profile.Name);

                return RedirectToAction("Profile");
            }
            TempData["LoginError"] = "Zle prihlasovacie udaje";
            return RedirectToAction("LogIn");
        }

        public IActionResult Profile()
        {
            if (!Request.Cookies.ContainsKey("UserID") || !int.TryParse(Request.Cookies["UserID"], out int userID))
            {
                return RedirectToAction("LogIn");
            }
            var userName = Request.Cookies["UserName"];
            //ViewBag.UserID = userID;
            ViewBag.UserName = userName;
            var model = new ProfileViewModel
            {
                Character = _drdContext.Characters.Where(c => c.UserID == userID).ToList(),
                Dungeon = _drdContext.Dungeon.Where(c => c.UserID == userID).ToList()
            };
            

            return View(model);
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserName");
            return RedirectToAction("LogIn");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]

        public async Task<ActionResult> Register([Bind("Name, Surname, Nickname, UserID, Password")] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user as UserModel);
            }
            _drdContext.Users.Add(user);
            await _drdContext.SaveChangesAsync();
            return RedirectToAction("LogIn");
        }
        [HttpGet]
        public IActionResult NewCharacter()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> NewCharacter(Character character)
        {
            if (!ModelState.IsValid)
            {
                return View(character);
            }
            var userID = int.Parse(Request.Cookies["UserID"]);
            character.UserID = userID;
            //character.DunID = null;
            //if (character.CharHitPointsMax == 0)
            //{
            //    ModelState.AddModelError("", "Maximalne zivoty nemozu byt 0");
            //    return View(character);
            //}

            if(character.CharManaMax == null)
            {
                character.CharManaMax = 0;
            }
            character.CharMana = character.CharManaMax;
            character.CharHitPoints = character.CharHitPointsMax;

            //character.CharExperiencePoints ??= 0;

            _drdContext.Characters.Add(character);
            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult AddAtributes(int CharID)
        {
            var atributesExists = _drdContext.Characters.Find(CharID);
            if (atributesExists.CharAgility != null || atributesExists.CharEndurance != null ||
                atributesExists.CharCharisma != null || atributesExists.CharInteligent != null || atributesExists.CharStrenght != null)
            {
                TempData["AtributeExists"] = "Atributy uz existuju pre tuto postavu";
                return RedirectToAction("Profile");
            }
            Response.Cookies.Append("CharID", CharID.ToString(), new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(1),
                HttpOnly = true,
                IsEssential = true
            });

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddAtributes( Character character)
        {
            if (!ModelState.IsValid)
            {
                return View(character);
            }
            if (!int.TryParse(Request.Cookies["CharID"], out int charID))
                return BadRequest("CharID cookie neexistuje/neplatna.");

            var existing = await _drdContext.Characters.FindAsync(charID);
            if (existing == null)
            {
                return NotFound("Postava sa nenasla");
            }
            existing.CharAgility = character.CharAgility;
            existing.CharInteligent = character.CharInteligent;
            existing.CharStrenght = character.CharStrenght;
            existing.CharCharisma = character.CharCharisma;
            existing.CharEndurance = character.CharEndurance;

            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult CharacterOverView(int charID)
        {
            var character = _drdContext.Characters.FirstOrDefault(c => c.CharacterId == charID);
            if (character == null) return NotFound();
            return View(character);
        }

        [HttpPost]
        public async Task<ActionResult> SavingChangesChar(IFormCollection form)
        {
            if (!int.TryParse(form["CharacterID"], out int charID))
            {
                return BadRequest("CharacterID parameter je neplatny alebo chyba");
            }

            var change = await _drdContext.Characters.FindAsync(charID);
            if(change == null)
            {
                return NotFound("Postava nebola najdena");
            }

            change.CharHitPoints = int.Parse(form["CharHitPoints"]);
            change.CharLevel = int.Parse(form["CharLevel"]);
            change.CharExperiencePoints = int.Parse(form["CharExperiencePoints"]);
            change.CharMana = int.Parse(form["CharMana"]);
            change.CharWeapons = form["CharWeapons"];
            change.CharItems = form["CharItems"];
            change.CharDefense = form["CharDefense"];

            await _drdContext.SaveChangesAsync();

            if (int.Parse(form["UserType"]) == 1)
            {
                return RedirectToAction("CharacterOverView", new { charID = charID });
            }
            else
            {
                if (!int.TryParse(form["DungeonID"], out int dunID))
                {
                    return BadRequest("DungeonID parameter je neplatny alebo chyba");
                }
                return RedirectToAction("WorldCharacters", new {DunID  = dunID});
            }
            
            
            
            
        }

        public IActionResult CreateWorld()
        {
            if (TempData["CreatingError"] != null)
            {
                TempData.Remove("CreatingError");
            }
            return View(); 
        }

        [HttpPost]
        public async Task<ActionResult> CreateWorld(Dungeon dungeon)
        {
            if (!int.TryParse(Request.Cookies["UserID"], out int userID))
            {
                return BadRequest("UserID cookie neexistuje/neplatna");
            }
            if (!ModelState.IsValid)
            {
                TempData["CreatingError"] = "Treba zadat meno sveta";
                return RedirectToAction("CreateWorld");
            }
            dungeon.UserID = userID;

             _drdContext.Dungeon.Add(dungeon);
            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult AddCharacters(int DunID)
        {
            var unassignedCharacters = _drdContext.Characters
                .Where(c => c.DunID == null)
                .ToList();

            var viewModel = new AddCharactersViewModel
            {
                DunID = DunID,
                AvailableCharacters = unassignedCharacters
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacters(AddCharactersViewModel model)
        {
            var charactersToUpdate = _drdContext.Characters
                .Where(c => model.SelectedCharacterIds.Contains(c.CharacterId))
                .ToList();

            foreach(var character in charactersToUpdate)
            {
                character.DunID = model.DunID;
            }
            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult WorldCharacters(int DunID)
        {
            var thisWorld = _drdContext.Characters
                .Where(c => c.DunID == DunID)
                .ToList();

            return View(thisWorld);
        }
    }
}