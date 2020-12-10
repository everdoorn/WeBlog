﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class PostList : BlogRenderingModelBase
    {
        protected const string DefaultPostTemplate = "~/Views/WeBlog/PostListEntry.cshtml";

        public IEnumerable<EntryItem> Entries { get; set; }
        public bool ShowViewMoreLink { get; set; }
        public string ViewMoreHref { get; set; }
        public string PostTemplate { get; set; }
        public ICommentManager CommentManager { get; protected set; }

        protected IPostListCore PostListCore { get; set; }

        public PostList() : this(null, null) { }

        public PostList(IPostListCore postListCore, ICommentManager commentManager)
        {
            PostListCore = postListCore ?? new PostListCore(CurrentBlog, null, null, null, null);
            CommentManager = commentManager ?? ServiceLocator.ServiceProvider.GetRequiredService<ICommentManager>();
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var queryString = RenderingContext.Current.PageContext.RequestContext.HttpContext.Request.QueryString;
            PostListCore.Initialize(queryString);
            if (string.IsNullOrEmpty(PostTemplate))
            {
                PostTemplate = DefaultPostTemplate;
            }

            Entries = PostListCore.Entries;
            PostTemplate = PostTemplate;

            if (PostListCore.ShowViewMoreLink)
            {
                ShowViewMoreLink = true;
                ViewMoreHref = PostListCore.ViewMoreHref;
            }
            else
            {
                ShowViewMoreLink = false;
            }
        }
    }
}