using BigSchool.Models;
using BigSchool.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BigSchool.Controllers.Api
{
    [Authorize]
    public class FollowingsController : ApiController
    {
        private ApplicationDbContext Context => HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();

        [HttpPost]
        public async Task<IHttpActionResult> Follow(FollowingViewModel model)
        {
            string userId = User.Identity.GetUserId();

            if (Context.Followings.Any(f => f.FollowerId == userId && f.FolloweeId == model.FolloweeId))
                return BadRequest("Following already existed");

            Following following = new Following()
            {
                FollowerId = userId,
                FolloweeId = model.FolloweeId
            };

            Context.Followings.Add(following);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Unfollow(FollowingViewModel model)
        {
            string userId = User.Identity.GetUserId();

            if (!Context.Followings.Any(f => f.FollowerId == userId && f.FolloweeId == model.FolloweeId))
                return BadRequest("You've not followed this lecturer yet!");

            Following following =
                await Context.Followings.FirstOrDefaultAsync(f =>
                    f.FollowerId == userId && f.FolloweeId == model.FolloweeId);
            Context.Followings.Remove(following);

            await Context.SaveChangesAsync();

            return Ok();
        }
    }
}
