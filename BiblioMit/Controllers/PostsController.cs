using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BiblioMit.Models;
using BiblioMit.Models.PostViewModels;
using BiblioMit.Models.ReplyViewModels;
using Microsoft.AspNetCore.Identity;
using BiblioMit.Services;
using Microsoft.AspNetCore.Authorization;

namespace BiblioMit.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPost _postService;
        private readonly IAppUser _userService;
        private readonly IForum _forumService;
        private static UserManager<AppUser> _userManager;

        public PostsController(
            IPost postService,
            IForum forumService,
            IAppUser userService,
            UserManager<AppUser> userManager)
        {
            _postService = postService;
            _forumService = forumService;
            _userManager = userManager;
            _userService = userService;
        }
        
        public IActionResult Index(int Id)
        {
            var post = _postService.GetById(Id);

            var replies = BuildPostReplies(post.Replies);

            var model = new PostIndexModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.User.Id,
                AuthorEmail = post.User.Email,
                AuthorName = post.User.Name+" "+post.User.Last,
                AuthorRating = post.User.Rating,
                Created = post.Created,
                PostContent = post.Content,
                Replies = replies,
                ForumId = post.ForumId,
                ForumName = post.Forum.Title,
                AuthorImageUrl = post.User.ProfileImageUrl
            };
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int id)
        {
            var forum = _forumService.GetbyId(id);
            var model = new NewPostModel
            {
                ForumName = forum.Title,
                ForumId = forum.Id,
                ForumImageUrl = forum.ImageUrl,
                AuthorName = User.Identity.Name
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPost(NewPostModel model)
        {
            if (model == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var user = _userManager.FindByIdAsync(userId).Result;
            var post = BuildPost(model, user);

            await _postService.Add(post).ConfigureAwait(false);

            await _userService.UpdateUserRating(userId, typeof(Post)).ConfigureAwait(false);

            return RedirectToAction("Index", "Posts", new { id = post.Id } );
        }

        private static Post BuildPost(NewPostModel model, AppUser user)
        {
            return new Post
            {
                Title = model.Title,
                Content = model.Content,
                Created = DateTime.Now,
                User = user,
                ForumId = model.ForumId
            };
        }

        private static IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply> replies)
        {
            return replies.Select(r => new PostReplyModel
            {
                Id = r.Id,
                AuthorName = r.User.UserName,
                AuthorId = r.User.Id,
                AuthorImageUrl = r.User.ProfileImageUrl,
                AuthorRating = r.User.Rating,
                Created = r.Created,
                ReplyContent = r.Content
            });
        }
    }
}