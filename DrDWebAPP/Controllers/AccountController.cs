using DrDWebAPP.Data;
using DrDWebAPP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.ComponentModel;
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
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> NewCharacter(CharacterInfo characterInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(characterInfo);
            }
            var userID = int.Parse(Request.Cookies["UserID"]);

            var character = new Character
            {
                UserID = userID,
                CharName = characterInfo.CharName,
                CharRace = characterInfo.CharRace,
                CharProfession = characterInfo.CharProfession,
                CharLevel = characterInfo.CharLevel,
                CharExperiencePoints = characterInfo.CharExperiencePoints,
                CharHitPointsMax = characterInfo.CharHitPointsMax,
                CharHitPoints = characterInfo.CharHitPointsMax,
                CharManaMax = characterInfo.CharManaMax,
                CharMana = characterInfo.CharManaMax
            };

            _drdContext.Characters.Add(character);
            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult AddAtributes(int CharID)
        {
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;

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
            //var professionAtt = _drdContext.ProfessionAttributes.Find(atributesExists.CharProfession);
            //var raceAtt = _drdContext.RaceAttributes.Find(atributesExists.CharRace);
            //var attMod = _drdContext.AttributesModifiers.Find(atributesExists.CharRace);

            //string[] attList = ["Strength", "Endurance", "Dexterity", "Charisma", "Intelligence"];
            
            ////Dictionary<string, int[]> attValues = new Dictionary<string, int[]>();

            ////var professionAtt = _drdContext.ProfessionAttributes.Find(atributesExists.CharProfession);

            //var attValuesDic = new Dictionary<string, int[]>();

            //if (professionAtt != null)
            //{
            //    var properties = professionAtt.GetType().GetProperties();

            //    foreach (var prop in properties)
            //    {
            //        if (attList.Contains(prop.Name))
            //        {
            //            var value = prop.GetValue(professionAtt)?.ToString();

            //            if (!string.IsNullOrWhiteSpace(value) && value.ToLower() != "null")
            //            {
            //                try
            //                {
            //                    var parts = value.Split('-');
            //                    if (parts.Length == 2 &&
            //                        int.TryParse(parts[0], out int from) &&
            //                        int.TryParse(parts[1], out int to))
            //                    {
            //                        attValuesDic.Add(prop.Name, Enumerable.Range(from, to - from + 1).ToArray());
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    Console.WriteLine($"Chyba pri spracovaní atribútu {prop.Name}: {ex.Message}");
            //                }
            //            }
            //        }
            //    }
            //}

            //foreach (var att in attList)
            //{
            //    if(attValuesDic.ContainsKey(att))
            //    {
            //        attValuesDic.
            //    }
            //}



            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddAtributes( CharacterAttr characterAttr)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Horste mame problem");
                return View(characterAttr);
            }
            if (!int.TryParse(Request.Cookies["CharID"], out int charID))
                return BadRequest("CharID cookie neexistuje/neplatna.");

            var existing = await _drdContext.Characters.FindAsync(charID);
            if (existing == null)
            {
                return NotFound("Postava sa nenasla");
            }
            var attModel = await _drdContext.Attributes.FirstOrDefaultAsync(x => x.RaceAttributesID == existing.CharRace && x.ProfessionAttributesID == existing.CharProfession);

            existing.CharAgility = characterAttr.CharAgility;
            existing.CharInteligent = characterAttr.CharInteligent;
            existing.CharStrenght = characterAttr.CharStrenght;
            existing.CharCharisma = characterAttr.CharCharisma;
            existing.CharEndurance = characterAttr.CharEndurance;

            await _drdContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult CharacterOverView(int charID)
        {
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;

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

            bool isError = false;

            if(change.CharMana > change.CharManaMax)
            {
                TempData["ManaError"] = "Aktuálna mana nesmie byť väčšia ako maximálna mana";
                TempData["CharName"] = change.CharName;
                isError = true;
            }

            if(change.CharHitPoints > change.CharHitPointsMax)
            {
                TempData["HitPointsError"] = "Aktuálne životy nesmú byť väčšie ako maximálne životy";
                TempData["CharName"] = change.CharName;
                isError = true;
            }
            if (!isError)
            {
                await _drdContext.SaveChangesAsync();
                TempData["CharName"] = null;
            }

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
                return RedirectToAction("WorldCharacters", new { DunID = dunID });
            }
            
        }

        public IActionResult CreateWorld()
        {
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;

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
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;

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
            var userName = Request.Cookies["UserName"];
            ViewBag.UserName = userName;

            var thisWorld = _drdContext.Characters
                .Where(c => c.DunID == DunID)
                .ToList();

            return View(thisWorld);
        }
    }
}