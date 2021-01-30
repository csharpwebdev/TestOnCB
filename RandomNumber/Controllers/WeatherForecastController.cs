using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RandomNumber.Data;
using RandomNumber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RandomNumber.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MatchController(ILogger<MatchController> logger, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = dbContext;
            _userManager = userManager;
        }

        [HttpGet("getpast")]
        [AllowAnonymous]
        public IEnumerable<MatchResult> GetPastMatches()
        {
            var pastMatches = _db.Matches.Where(m => m.ExpiryDate < DateTime.UtcNow).OrderByDescending(m => m.ExpiryDate).ToList();

            var results = pastMatches
                .Select(m => {
                    var winner = _db.UserMatches.Where(um => um.MatchId == m.Id).OrderByDescending(um => um.ResultNumber).FirstOrDefault();
                    if(winner != null)
                        return new MatchResult() { MatchName = m.Name, WinnerName = _db.Users.FirstOrDefault(u => u.UserName == winner.UserId).UserName, WinnerValue = winner.ResultNumber.ToString() };
                    else
                        return new MatchResult() { MatchName = m.Name, WinnerName = "N/A", WinnerValue = "N/A" };
                }).ToList();

            return results.ToArray();
        }

        [HttpGet("current")]
        public async Task<CurrentMatchResult> GetCurrentMatch()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            var curMatch = _db.Matches.FirstOrDefault(m => m.ExpiryDate >= DateTime.UtcNow);
            CurrentMatchResult result = new CurrentMatchResult();

            if(curMatch == null)
            {
                result.MatchId = 0;
                result.MatchName = "N/A";
                result.YourNumber = "N/A";
                result.AlreadyPlayed = false;
            }
            else
            {
                result.MatchId = curMatch.Id;
                result.MatchName = curMatch.Name;
                result.ExpiryDate = curMatch.ExpiryDate;

                var userMatch = _db.UserMatches.FirstOrDefault(um => um.UserId == user.UserName && um.MatchId == curMatch.Id);
                if(userMatch == null)
                {
                    // not played yet
                    result.AlreadyPlayed = false;
                }
                else
                {
                    // already played
                    result.AlreadyPlayed = true;
                    result.YourNumber = userMatch.ResultNumber.ToString();
                }
            }

            return result;
        }

        [HttpPost("play")]
        public async Task<CurrentMatchResult> Play()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            var curMatch = _db.Matches.FirstOrDefault(m => m.ExpiryDate >= DateTime.UtcNow);
            CurrentMatchResult result = new CurrentMatchResult();

            if (curMatch == null)
            {
                result.MatchId = 0;
            }
            else
            {
                result.MatchId = curMatch.Id;
                result.ExpiryDate = curMatch.ExpiryDate;

                var userMatch = _db.UserMatches.FirstOrDefault(um => um.UserId == user.UserName && um.MatchId == curMatch.Id);
                if (userMatch == null)
                {
                    // not played yet
                    Random rand = new Random(DateTime.Now.Millisecond);
                    userMatch = new UserMatch() { UserId = user.UserName, MatchId = curMatch.Id, ResultNumber = rand.Next(100) };
                    _db.UserMatches.Add(userMatch);
                    _db.SaveChanges();
                }

                // already played
                result.AlreadyPlayed = true;
                result.YourNumber = userMatch.ResultNumber.ToString();
            }

            return result;
        }

    }

    public class MatchResult
    {
        public string MatchName { get; set; }

        public string WinnerName { get; set; }

        public string WinnerValue { get; set; }
    }

    public class CurrentMatchResult
    {
        public int MatchId { get; set; }

        public string MatchName { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string YourNumber { get; set; }

        public bool AlreadyPlayed { get; set; }
    }
}
