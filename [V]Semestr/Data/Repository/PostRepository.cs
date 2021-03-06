﻿using _V_Semestr.Helpers;
using _V_Semestr.Models;
using _V_Semestr.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _V_Semestr.Data.Repository
{
    public class PostRepository : IPostRepository
    {
        private AppDbContext _ctx;
        public PostRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post); 
        }

        //public IndexViewModel GetAllPosts(int pageNumber)
        //{
        //    int pageSize = 5;
        //    int skip = pageSize * (pageNumber - 1);
        //    return new IndexViewModel
        //    {
        //        NextPage =  _ctx.Posts.Count() > skip + pageSize,
        //        Posts = _ctx.Posts
        //                .Skip(pageSize * (pageNumber - 1))
        //                .Take(pageSize)
        //                .ToList(),
        //        PageNumber = pageNumber
        //    };

        //}
        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                    //.ThenInclude(mc => mc.SubComments)
                .FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }
        public void UpdatePost(Post post)
        {
            int count = _ctx.ChangeTracker.Entries().Count();
            System.Diagnostics.Debug.WriteLine($"{count}");
            post.Updated = DateTime.Now;
            _ctx.Posts.Update(post);
        }
        public async Task<bool> SaveChangesAsync()
        {
            if(await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public IndexViewModel GetAllPosts(int pageNumber, string category, string search)
        {
            if (pageNumber < 1) pageNumber = 1;
            int pageSize = 2;
            int skip = pageSize * (pageNumber - 1);
            var query = _ctx.Posts
                .Include(p => p.Category)
                .Where(p => p.Shown)
                .AsNoTracking()
                .AsQueryable();
            if (!String.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name.ToLower().Equals(category.ToLower()));
            }
            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                                        || EF.Functions.Like(x.Content, $"%{search}%")
                                        || EF.Functions.Like(x.Desciption, $"%{search}%")
                                        || EF.Functions.Like(x.Tags, $"%{search}%"));
            }
            var postCount = query.Count();
            var pageCount = (int)Math.Ceiling((double)postCount / pageSize);
            if (pageNumber > pageCount && pageCount != 0) pageNumber = pageCount;
            var posts = query
                        .Skip(pageSize * (pageNumber - 1))
                        .Take(pageSize)
                        .ToList();
            return new IndexViewModel
            {
                NextPage = postCount > skip + pageSize,
                PageCount = pageCount,
                Posts = posts,
                Pages = PageHelper.GetPageNumbers(pageNumber, pageCount).ToList(),
                PageNumber = pageNumber,
                Category = category,
                Search = search,
            };
        }


        public void AddComment(Comment comment)
        {
           _ctx.Comments.Add(comment);
        }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            return _ctx.Comments
                    .Where(c => c.PostId == postId)
                    .AsNoTracking()
                    .ToList();
        }
    }
}
