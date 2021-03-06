﻿using Reddit.Controllers.Internal;
using Reddit.Exceptions;
using Reddit.Inputs.Flair;
using Reddit.Inputs.LinksAndComments;
using Reddit.Inputs.Moderation;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reddit.Controllers
{
    /// <summary>
    /// Base controller for posts.
    /// </summary>
    public class Post : BaseController
    {
        public string Subreddit;
        public string Author;
        public string Id;
        public string Fullname;
        public string Permalink;
        public DateTime Created;
        public DateTime Edited;
        public int Score;
        public int UpVotes;
        public int DownVotes;
        public bool Removed;
        public bool Spam;
        public bool NSFW;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = Parsing.HtmlDecode(value);
            }
        }
        private string title;

        /// <summary>
        /// The full Listing object returned by the Reddit API;
        /// </summary>
        public Things.Post Listing;

        /// <summary>
        /// Comment replies to this post.
        /// </summary>
        public Comments Comments
        {
            get
            {
                return comments ?? InitComments();
            }
            private set
            {
                comments = value;
            }
        }
        private Comments comments = null;

        internal Dispatch Dispatch;

        /// <summary>
        /// Create a new post controller instance from API return data.
        /// </summary>
        /// <param name="dispatch"></param>
        /// <param name="listing"></param>
        public Post(Dispatch dispatch, Things.Post listing)
        {
            Dispatch = dispatch;
            Import(listing);
        }

        /// <summary>
        /// Create a new post controller instance, populated manually.
        /// </summary>
        /// <param name="dispatch"></param>
        /// <param name="subreddit">The subreddit to which the post belongs</param>
        /// <param name="title">The title of the post</param>
        /// <param name="author">The post author's username</param>
        /// <param name="id"></param>
        /// <param name="fullname"></param>
        /// <param name="permalink"></param>
        /// <param name="created"></param>
        /// <param name="edited"></param>
        /// <param name="score"></param>
        /// <param name="upVotes"></param>
        /// <param name="downVotes"></param>
        /// <param name="removed"></param>
        /// <param name="spam"></param>
        /// <param name="nsfw"></param>
        public Post(Dispatch dispatch, string subreddit, string title = null, string author = null, string id = null, string fullname = null, string permalink = null,
            DateTime created = default(DateTime), DateTime edited = default(DateTime), int score = 0, int upVotes = 0,
            int downVotes = 0, bool removed = false, bool spam = false, bool nsfw = false)
        {
            Dispatch = dispatch;
            Import(subreddit, title, author, id, fullname, permalink, created, edited, score, upVotes, downVotes, removed, spam, nsfw);
        }

        /// <summary>
        /// Create a new post controller instance, populated with only its fullname.
        /// </summary>
        /// <param name="dispatch"></param>
        /// <param name="fullname">Fullname of the post</param>
        public Post(Dispatch dispatch, string fullname)
        {
            Dispatch = dispatch;
            Fullname = fullname;
        }

        /// <summary>
        /// Create a new post controller instance, populated with only its fullname and subreddit.
        /// </summary>
        /// <param name="dispatch"></param>
        /// <param name="fullname">Fullname of the post</param>
        /// <param name="subreddit">A valid subreddit name</param>
        public Post(Dispatch dispatch, string fullname, string subreddit)
        {
            Dispatch = dispatch;
            Fullname = fullname;
            Subreddit = subreddit;
        }

        /// <summary>
        /// Create an empty post controller instance.
        /// </summary>
        /// <param name="dispatch"></param>
        public Post(Dispatch dispatch)
        {
            Dispatch = dispatch;
        }

        private Comments InitComments()
        {
            Comments = new Comments(Dispatch, Id, Subreddit);
            return Comments;
        }

        internal void Import(Things.Post listing)
        {
            Subreddit = listing.Subreddit;
            Title = listing.Title;
            Author = listing.Author;
            Id = listing.Id;
            Fullname = listing.Name;
            Permalink = listing.Permalink;
            Created = listing.Created;
            Edited = listing.Edited;
            Score = listing.Score;
            UpVotes = listing.Ups;
            DownVotes = listing.Downs;
            Removed = listing.Removed;
            Spam = listing.Spam;
            NSFW = listing.Over18;

            Listing = listing;
        }

        internal void Import(string subreddit, string title, string author, string id = null, string fullname = null, string permalink = null,
            DateTime created = default(DateTime), DateTime edited = default(DateTime), int score = 0, int upVotes = 0,
            int downVotes = 0, bool removed = false, bool spam = false, bool nsfw = false)
        {
            Subreddit = subreddit;
            Title = title;
            Author = author;
            Id = id;
            Fullname = fullname;
            Permalink = permalink;
            Created = created;
            Edited = edited;
            Score = score;
            UpVotes = upVotes;
            DownVotes = downVotes;
            Removed = removed;
            Spam = spam;
            NSFW = nsfw;

            Listing = new Things.Post(this);
        }

        /// <summary>
        /// Set the title manually without any automatic decoding.
        /// </summary>
        /// <param name="value">The title value you wish to set</param>
        public void SetTitle(string value)
        {
            title = value;
        }

        /// <summary>
        /// Create a new comment controller instance bound to this post, populated manually.
        /// </summary>
        /// <param name="body">The comment text</param>
        /// <param name="bodyHtml"></param>
        /// <param name="author">The username of the comment's author</param>
        /// <param name="collapsedReason"></param>
        /// <param name="collapsed"></param>
        /// <param name="isSubmitter"></param>
        /// <param name="replies"></param>
        /// <param name="scoreHidden"></param>
        /// <param name="depth"></param>
        /// <param name="id"></param>
        /// <param name="fullname"></param>
        /// <param name="permalink"></param>
        /// <param name="created"></param>
        /// <param name="edited"></param>
        /// <param name="score"></param>
        /// <param name="upVotes"></param>
        /// <param name="downVotes"></param>
        /// <param name="removed"></param>
        /// <param name="spam"></param>
        /// <returns></returns>
        public Comment Comment(string body, string bodyHtml = null, string author = null, 
            string collapsedReason = null, bool collapsed = false, bool isSubmitter = false,
            List<Comment> replies = null, bool scoreHidden = false, int depth = 0, string id = null, string fullname = null,
            string permalink = null, DateTime created = default(DateTime), DateTime edited = default(DateTime),
            int score = 0, int upVotes = 0, int downVotes = 0, bool removed = false, bool spam = false)
        {
            return new Comment(Dispatch, Subreddit, author, body, Fullname, bodyHtml, collapsedReason, collapsed, isSubmitter, replies, scoreHidden,
                depth, id, fullname, permalink, created, edited, score, upVotes, downVotes, removed, spam);
        }

        /// <summary>
        /// Create a new comment controller instance bound to this post.
        /// </summary>
        /// <returns></returns>
        public Comment Comment()
        {
            return new Comment(Dispatch, Subreddit, null, null, Fullname);
        }

        /// <summary>
        /// Reply with a comment to this post.
        /// </summary>
        /// <param name="body">The comment reply text</param>
        /// <param name="bodyHtml"></param>
        /// <param name="author"></param>
        /// <param name="collapsedReason"></param>
        /// <param name="collapsed"></param>
        /// <param name="isSubmitter"></param>
        /// <param name="replies"></param>
        /// <param name="scoreHidden"></param>
        /// <param name="depth"></param>
        /// <param name="id"></param>
        /// <param name="fullname"></param>
        /// <param name="permalink"></param>
        /// <param name="created"></param>
        /// <param name="edited"></param>
        /// <param name="score"></param>
        /// <param name="upVotes"></param>
        /// <param name="downVotes"></param>
        /// <param name="removed"></param>
        /// <param name="spam"></param>
        /// <returns>The newly-created comment reply.</returns>
        public Comment Reply(string body, string bodyHtml = null, string author = null,
            string collapsedReason = null, bool collapsed = false, bool isSubmitter = false,
            List<Comment> replies = null, bool scoreHidden = false, int depth = 0, string id = null, string fullname = null,
            string permalink = null, DateTime created = default(DateTime), DateTime edited = default(DateTime),
            int score = 0, int upVotes = 0, int downVotes = 0, bool removed = false, bool spam = false)
        {
            return Comment(body, bodyHtml, author, collapsedReason, collapsed, isSubmitter, replies, scoreHidden,
                depth, id, fullname, permalink, created, edited, score, upVotes, downVotes, removed, spam).Submit();
        }

        /// <summary>
        /// Reply with a comment to this post asynchronously.
        /// </summary>
        /// <param name="body">The comment reply text</param>
        /// <param name="bodyHtml"></param>
        /// <param name="author"></param>
        /// <param name="collapsedReason"></param>
        /// <param name="collapsed"></param>
        /// <param name="isSubmitter"></param>
        /// <param name="replies"></param>
        /// <param name="scoreHidden"></param>
        /// <param name="depth"></param>
        /// <param name="id"></param>
        /// <param name="fullname"></param>
        /// <param name="permalink"></param>
        /// <param name="created"></param>
        /// <param name="edited"></param>
        /// <param name="score"></param>
        /// <param name="upVotes"></param>
        /// <param name="downVotes"></param>
        /// <param name="removed"></param>
        /// <param name="spam"></param>
        public async Task<Comment> ReplyAsync(string body, string bodyHtml = null, string author = null,
            string collapsedReason = null, bool collapsed = false, bool isSubmitter = false,
            List<Comment> replies = null, bool scoreHidden = false, int depth = 0, string id = null, string fullname = null,
            string permalink = null, DateTime created = default(DateTime), DateTime edited = default(DateTime),
            int score = 0, int upVotes = 0, int downVotes = 0, bool removed = false, bool spam = false)
        {
            return await Comment(body, bodyHtml, author, collapsedReason, collapsed, isSubmitter, replies, scoreHidden,
                depth, id, fullname, permalink, created, edited, score, upVotes, downVotes, removed, spam).SubmitAsync();
        }

        /// <summary>
        /// Return information about the current Post instance.
        /// </summary>
        /// <returns>An instance of this class populated with the retrieved data.</returns>
        public Post About()
        {
            Info info = Validate(Dispatch.LinksAndComments.Info(Fullname, Subreddit));
            if (info == null
                || info.Posts == null
                || info.Posts.Count == 0
                || !Fullname.Equals(info.Posts[0].Name))
            {
                throw new RedditControllerException("Unable to retrieve post data.");
            }

            return new Post(Dispatch, info.Posts[0]);
        }

        /// <summary>
        /// Distinguish a post's author with a sigil.
        /// This can be useful to draw attention to and confirm the identity of the user in the context of a link of theirs.
        /// The options for distinguish are as follows:
        /// yes - add a moderator distinguish([M]). only if the user is a moderator of the subreddit the thing is in.
        /// no - remove any distinguishes.
        /// admin - add an admin distinguish([A]). admin accounts only.
        /// special - add a user-specific distinguish. depends on user.
        /// </summary>
        /// <param name="how">one of (yes, no, admin, special)</param>
        /// <returns>The distinguished post object.</returns>
        public Post Distinguish(string how)
        {
            return base.Lists.GetPosts(Validate(Dispatch.Moderation.DistinguishPost(how, Fullname)), Dispatch)[0];
        }

        /// <summary>
        /// Distinguish a post's author with a sigil asynchronously.
        /// This can be useful to draw attention to and confirm the identity of the user in the context of a link of theirs.
        /// The options for distinguish are as follows:
        /// yes - add a moderator distinguish([M]). only if the user is a moderator of the subreddit the thing is in.
        /// no - remove any distinguishes.
        /// admin - add an admin distinguish([A]). admin accounts only.
        /// special - add a user-specific distinguish. depends on user.
        /// </summary>
        /// <param name="how">one of (yes, no, admin, special)</param>
        /// <returns>The distinguished post object.</returns>
        public async Task<Post> DistinguishAsync(string how)
        {
            return base.Lists.GetPosts(Validate(await Dispatch.Moderation.DistinguishPostAsync(how, Fullname)), Dispatch)[0];
        }

        /// <summary>
        /// Remove this post from all subreddit listings.
        /// </summary>
        /// <param name="spam">boolean value</param>
        public void Remove(bool spam = false)
        {
            Dispatch.Moderation.Remove(new ModerationRemoveInput(Fullname, spam));
        }

        /// <summary>
        /// Asynchronously remove this post from all subreddit listings.
        /// </summary>
        public async Task RemoveAsync(bool spam = false)
        {
            await Dispatch.Moderation.RemoveAsync(new ModerationRemoveInput(Fullname, spam));
        }

        /// <summary>
        /// Delete this post.
        /// </summary>
        public void Delete()
        {
            Dispatch.LinksAndComments.Delete(Fullname);
        }

        /// <summary>
        /// Delete this post asynchronously.
        /// </summary>
        public async Task DeleteAsync()
        {
            await Dispatch.LinksAndComments.DeleteAsync(Fullname);
        }

        /// <summary>
        /// Hide this post.
        /// This removes it from the user's default view of subreddit listings.
        /// </summary>
        public void Hide()
        {
            Dispatch.LinksAndComments.Hide(Fullname);
        }

        /// <summary>
        /// Hide this post asynchronously.
        /// This removes it from the user's default view of subreddit listings.
        /// </summary>
        public async Task HideAsync()
        {
            await Dispatch.LinksAndComments.HideAsync(Fullname);
        }

        /// <summary>
        /// Lock this post.
        /// Prevents a post from receiving new comments.
        /// </summary>
        public void Lock()
        {
            Dispatch.LinksAndComments.Lock(Fullname);
        }

        /// <summary>
        /// Lock this post asynchronously.
        /// Prevents a post from receiving new comments.
        /// </summary>
        public async Task LockAsync()
        {
            await Dispatch.LinksAndComments.LockAsync(Fullname);
        }

        /// <summary>
        /// Mark this post as NSFW.
        /// </summary>
        public void MarkNSFW()
        {
            Dispatch.LinksAndComments.MarkNSFW(Fullname);
        }

        /// <summary>
        /// Mark this post as NSFW asynchronously.
        /// </summary>
        public async Task MarkNSFWAsync()
        {
            await Dispatch.LinksAndComments.MarkNSFWAsync(Fullname);
        }

        /// <summary>
        /// Retrieve additional comments omitted from a base comment tree.
        /// When a comment tree is rendered, the most relevant comments are selected for display first.
        /// Remaining comments are stubbed out with "MoreComments" links. 
        /// This API call is used to retrieve the additional comments represented by those stubs, up to 100 at a time.
        /// The two core parameters required are link and children. link is the fullname of the link whose comments are being fetched. 
        /// children is a comma-delimited list of comment ID36s that need to be fetched.
        /// If id is passed, it should be the ID of the MoreComments object this call is replacing. This is needed only for the HTML UI's purposes and is optional otherwise.
        /// NOTE: you may only make one request at a time to this API endpoint. Higher concurrency will result in an error being returned.
        /// If limit_children is True, only return the children requested.
        /// </summary>
        /// <param name="children">a comma-delimited list of comment ID36s</param>
        /// <param name="limitChildren">boolean value</param>
        /// <param name="sort">one of (confidence, top, new, controversial, old, random, qa, live)</param>
        /// <param name="id">(optional) id of the associated MoreChildren object</param>
        /// <returns>The requested comments.</returns>
        public MoreChildren MoreChildren(string children, bool limitChildren, string sort, string id = null)
        {
            return MoreChildren(new LinksAndCommentsMoreChildrenInput(children, limitChildren, Fullname, sort, id));
        }

        /// <summary>
        /// Retrieve additional comments omitted from a base comment tree.
        /// When a comment tree is rendered, the most relevant comments are selected for display first.
        /// Remaining comments are stubbed out with "MoreComments" links. 
        /// This API call is used to retrieve the additional comments represented by those stubs, up to 100 at a time.
        /// The two core parameters required are link and children. link is the fullname of the link whose comments are being fetched. 
        /// children is a comma-delimited list of comment ID36s that need to be fetched.
        /// If id is passed, it should be the ID of the MoreComments object this call is replacing. This is needed only for the HTML UI's purposes and is optional otherwise.
        /// NOTE: you may only make one request at a time to this API endpoint. Higher concurrency will result in an error being returned.
        /// If limit_children is True, only return the children requested.
        /// </summary>
        /// <param name="linksAndCommentsMoreChildrenInput">A valid LinksAndCommentsMoreChildrenInput instance</param>
        /// <returns>The requested comments.</returns>
        public MoreChildren MoreChildren(LinksAndCommentsMoreChildrenInput linksAndCommentsMoreChildrenInput)
        {
            linksAndCommentsMoreChildrenInput.link_id = Fullname;

            return Validate(Dispatch.LinksAndComments.MoreChildren(linksAndCommentsMoreChildrenInput));
        }

        /// <summary>
        /// Report this post to the subreddit moderators.  The post then becomes implicitly hidden, as well.
        /// </summary>
        /// <param name="additionalInfo">a string no longer than 2000 characters</param>
        /// <param name="banEvadingAccountsNames">a string no longer than 1000 characters</param>
        /// <param name="customText">a string no longer than 250 characters</param>
        /// <param name="fromHelpCenter">boolean value</param>
        /// <param name="otherReason">a string no longer than 100 characters</param>
        /// <param name="reason">a string no longer than 100 characters</param>
        /// <param name="ruleReason">a string no longer than 100 characters</param>
        /// <param name="siteReason">a string no longer than 100 characters</param>
        /// <param name="violatorUsername">A valid Reddit username</param>
        public void Report(string additionalInfo, string banEvadingAccountsNames, string customText, bool fromHelpCenter,
            string otherReason, string reason, string ruleReason, string siteReason, string violatorUsername)
        {
            Report(new LinksAndCommentsReportInput(additionalInfo, banEvadingAccountsNames, customText, fromHelpCenter, otherReason, reason,
                ruleReason, siteReason, Subreddit, Fullname, violatorUsername));
        }

        /// <summary>
        /// Report this post to the subreddit moderators asynchronously.  The post then becomes implicitly hidden, as well.
        /// </summary>
        /// <param name="additionalInfo">a string no longer than 2000 characters</param>
        /// <param name="banEvadingAccountsNames">a string no longer than 1000 characters</param>
        /// <param name="customText">a string no longer than 250 characters</param>
        /// <param name="fromHelpCenter">boolean value</param>
        /// <param name="otherReason">a string no longer than 100 characters</param>
        /// <param name="reason">a string no longer than 100 characters</param>
        /// <param name="ruleReason">a string no longer than 100 characters</param>
        /// <param name="siteReason">a string no longer than 100 characters</param>
        /// <param name="violatorUsername">A valid Reddit username</param>
        public async Task ReportAsync(string additionalInfo, string banEvadingAccountsNames, string customText, bool fromHelpCenter,
            string otherReason, string reason, string ruleReason, string siteReason, string violatorUsername)
        {
            await ReportAsync(new LinksAndCommentsReportInput(additionalInfo, banEvadingAccountsNames, customText, fromHelpCenter, otherReason, reason,
                ruleReason, siteReason, Subreddit, Fullname, violatorUsername));
        }

        /// <summary>
        /// Report this post to the subreddit moderators.  The post then becomes implicitly hidden, as well.
        /// </summary>
        /// <param name="linksAndCommentsReportInput">A valid LinksAndCommentsReportInput instance</param>
        public void Report(LinksAndCommentsReportInput linksAndCommentsReportInput)
        {
            Validate(Dispatch.LinksAndComments.Report(linksAndCommentsReportInput));
        }

        /// <summary>
        /// Report this post to the subreddit moderators asynchronously.  The post then becomes implicitly hidden, as well.
        /// </summary>
        /// <param name="linksAndCommentsReportInput">A valid LinksAndCommentsReportInput instance</param>
        public async Task ReportAsync(LinksAndCommentsReportInput linksAndCommentsReportInput)
        {
            Validate(await Dispatch.LinksAndComments.ReportAsync(linksAndCommentsReportInput));
        }

        /// <summary>
        /// Save this post.
        /// Saved things are kept in the user's saved listing for later perusal.
        /// </summary>
        /// <param name="category">a category name</param>
        public void Save(string category)
        {
            Dispatch.LinksAndComments.Save(new LinksAndCommentsSaveInput(Fullname, category));
        }

        /// <summary>
        /// Save this post asynchronously.
        /// Saved things are kept in the user's saved listing for later perusal.
        /// </summary>
        /// <param name="category">a category name</param>
        public async Task SaveAsync(string category)
        {
            await Dispatch.LinksAndComments.SaveAsync(new LinksAndCommentsSaveInput(Fullname, category));
        }

        /// <summary>
        /// Enable inbox replies for this post.
        /// </summary>
        public void EnableSendReplies()
        {
            Dispatch.LinksAndComments.SendReplies(new LinksAndCommentsStateInput(Fullname, true));
        }

        /// <summary>
        /// Enable inbox replies for this post asynchronously.
        /// </summary>
        public async Task EnableSendRepliesAsync()
        {
            await Dispatch.LinksAndComments.SendRepliesAsync(new LinksAndCommentsStateInput(Fullname, true));
        }

        /// <summary>
        /// Disable inbox replies for this post.
        /// </summary>
        public void DisableSendReplies()
        {
            Dispatch.LinksAndComments.SendReplies(new LinksAndCommentsStateInput(Fullname, false));
        }

        /// <summary>
        /// Disable inbox replies for this post asynchronously.
        /// </summary>
        public async Task DisableSendRepliesAsync()
        {
            await Dispatch.LinksAndComments.SendRepliesAsync(new LinksAndCommentsStateInput(Fullname, false));
        }

        /// <summary>
        /// Enable contest mode for this post.
        /// </summary>
        public void EnableContestMode()
        {
            Dispatch.LinksAndComments.SetContestMode(new LinksAndCommentsStateInput(Fullname, true));
        }

        /// <summary>
        /// Enable contest mode for this post asynchronously.
        /// </summary>
        public async Task EnableContestModeAsync()
        {
            await Dispatch.LinksAndComments.SetContestModeAsync(new LinksAndCommentsStateInput(Fullname, true));
        }

        /// <summary>
        /// Disable contest mode for this post.
        /// </summary>
        public void DisableContestMode()
        {
            Dispatch.LinksAndComments.SetContestMode(new LinksAndCommentsStateInput(Fullname, false));
        }

        /// <summary>
        /// Disable contest mode for this post asynchronously.
        /// </summary>
        public async Task DisableContestModeAsync()
        {
            await Dispatch.LinksAndComments.SetContestModeAsync(new LinksAndCommentsStateInput(Fullname, false));
        }

        /// <summary>
        /// Set this post as the sticky in its subreddit.
        /// The num argument is optional, and only used when stickying a post.
        /// It allows specifying a particular "slot" to sticky the post into, and if there is already a post stickied in that slot it will be replaced.
        /// If there is no post in the specified slot to replace, or num is None, the bottom-most slot will be used.
        /// </summary>
        /// <param name="num">an integer between 1 and 4</param>
        /// <param name="toProfile">boolean value</param>
        public void SetSubredditSticky(int num, bool toProfile)
        {
            Validate(Dispatch.LinksAndComments.SetSubredditSticky(new LinksAndCommentsStickyInput(Fullname, num, true, toProfile)));
        }

        /// <summary>
        /// Set this post as the sticky in its subreddit asynchronously.
        /// The num argument is optional, and only used when stickying a post.
        /// It allows specifying a particular "slot" to sticky the post into, and if there is already a post stickied in that slot it will be replaced.
        /// If there is no post in the specified slot to replace, or num is None, the bottom-most slot will be used.
        /// </summary>
        /// <param name="num">an integer between 1 and 4</param>
        /// <param name="toProfile">boolean value</param>
        public async Task SetSubredditStickyAsync(int num, bool toProfile)
        {
            Validate(await Dispatch.LinksAndComments.SetSubredditStickyAsync(new LinksAndCommentsStickyInput(Fullname, num, true, toProfile)));
        }

        /// <summary>
        /// Unset this post as the sticky in its subreddit.
        /// The num argument is optional, and only used when stickying a post.
        /// It allows specifying a particular "slot" to sticky the post into, and if there is already a post stickied in that slot it will be replaced.
        /// If there is no post in the specified slot to replace, or num is None, the bottom-most slot will be used.
        /// </summary>
        /// <param name="num">an integer between 1 and 4</param>
        /// <param name="toProfile">boolean value</param>
        public void UnsetSubredditSticky(int num, bool toProfile)
        {
            Validate(Dispatch.LinksAndComments.SetSubredditSticky(new LinksAndCommentsStickyInput(Fullname, num, false, toProfile)));
        }

        /// <summary>
        /// Unset this post as the sticky in its subreddit asynchronously.
        /// The num argument is optional, and only used when stickying a post.
        /// It allows specifying a particular "slot" to sticky the post into, and if there is already a post stickied in that slot it will be replaced.
        /// If there is no post in the specified slot to replace, or num is None, the bottom-most slot will be used.
        /// </summary>
        /// <param name="num">an integer between 1 and 4</param>
        /// <param name="toProfile">boolean value</param>
        public async Task UnsetSubredditStickyAsync(int num, bool toProfile)
        {
            Validate(await Dispatch.LinksAndComments.SetSubredditStickyAsync(new LinksAndCommentsStickyInput(Fullname, num, false, toProfile)));
        }

        /// <summary>
        /// Set a suggested sort for this post.
        /// Suggested sorts are useful to display comments in a certain preferred way for posts.
        /// For example, casual conversation may be better sorted by new by default, or AMAs may be sorted by Q&A.
        /// A sort of an empty string clears the default sort.
        /// </summary>
        /// <param name="sort">one of (confidence, top, new, controversial, old, random, qa, live, blank)</param>
        public void SetSuggestedSort(string sort)
        {
            Validate(Dispatch.LinksAndComments.SetSuggestedSort(new LinksAndCommentsSuggestedSortInput(Fullname, sort)));
        }

        /// <summary>
        /// Set a suggested sort for this post asynchronously.
        /// Suggested sorts are useful to display comments in a certain preferred way for posts.
        /// For example, casual conversation may be better sorted by new by default, or AMAs may be sorted by Q&A.
        /// A sort of an empty string clears the default sort.
        /// </summary>
        /// <param name="sort">one of (confidence, top, new, controversial, old, random, qa, live, blank)</param>
        public async Task SetSuggestedSortAsync(string sort)
        {
            Validate(await Dispatch.LinksAndComments.SetSuggestedSortAsync(new LinksAndCommentsSuggestedSortInput(Fullname, sort)));
        }

        /// <summary>
        /// Mark this post as containing spoilers.
        /// </summary>
        public void Spoiler()
        {
            Dispatch.LinksAndComments.Spoiler(Fullname);
        }

        /// <summary>
        /// Mark this post as containing spoilers asynchronously.
        /// </summary>
        public async Task SpoilerAsync()
        {
            await Dispatch.LinksAndComments.SpoilerAsync(Fullname);
        }

        /// <summary>
        /// Unhide this post.
        /// </summary>
        public void Unhide()
        {
            Dispatch.LinksAndComments.Unhide(Fullname);
        }

        /// <summary>
        /// Unhide this post asynchronously.
        /// </summary>
        public async Task UnhideAsync()
        {
            await Dispatch.LinksAndComments.UnhideAsync(Fullname);
        }

        /// <summary>
        /// Unlock this post.
        /// Allows this post to receive new comments.
        /// </summary>
        public void Unlock()
        {
            Dispatch.LinksAndComments.Unlock(Fullname);
        }

        /// <summary>
        /// Unlock this post asynchronously.
        /// Allows this post to receive new comments.
        /// </summary>
        public async Task UnlockAsync()
        {
            await Dispatch.LinksAndComments.UnlockAsync(Fullname);
        }

        /// <summary>
        /// Remove the NSFW marking from this post.
        /// </summary>
        public void UnmarkNSFW()
        {
            Dispatch.LinksAndComments.UnmarkNSFW(Fullname);
        }

        /// <summary>
        /// Remove the NSFW marking from this post asynchronously.
        /// </summary>
        public async Task UnmarkNSFWAsync()
        {
            await Dispatch.LinksAndComments.UnmarkNSFWAsync(Fullname);
        }

        /// <summary>
        /// Unsave this post.
        /// This removes the thing from the user's saved listings as well.
        /// </summary>
        public void Unsave()
        {
            Dispatch.LinksAndComments.Unsave(Fullname);
        }

        /// <summary>
        /// Unsave this post asynchronously.
        /// This removes the thing from the user's saved listings as well.
        /// </summary>
        public async Task UnsaveAsync()
        {
            await Dispatch.LinksAndComments.UnsaveAsync(Fullname);
        }

        /// <summary>
        /// Remove spoiler.
        /// </summary>
        public void Unspoiler()
        {
            Dispatch.LinksAndComments.Unspoiler(Fullname);
        }

        /// <summary>
        /// Remove spoiler asynchronously.
        /// </summary>
        public async Task UnspoilerAsync()
        {
            await Dispatch.LinksAndComments.UnspoilerAsync(Fullname);
        }

        /// <summary>
        /// Upvote this post.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public void Upvote()
        {
            Dispatch.LinksAndComments.Vote(new LinksAndCommentsVoteInput(Fullname, 1));
        }

        /// <summary>
        /// Upvote this post asynchronously.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public async Task UpvoteAsync()
        {
            await Dispatch.LinksAndComments.VoteAsync(new LinksAndCommentsVoteInput(Fullname, 1));
        }

        /// <summary>
        /// Downvote this post.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public void Downvote()
        {
            Dispatch.LinksAndComments.Vote(new LinksAndCommentsVoteInput(Fullname, -1));
        }

        /// <summary>
        /// Downvote this post asynchronously.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public async Task DownvoteAsync()
        {
            await Dispatch.LinksAndComments.VoteAsync(new LinksAndCommentsVoteInput(Fullname, -1));
        }

        /// <summary>
        /// Unvote this post.  This is equivalent to "un-voting" by clicking again on a highlighted arrow.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public void Unvote()
        {
            Dispatch.LinksAndComments.Vote(new LinksAndCommentsVoteInput(Fullname, 0));
        }

        /// <summary>
        /// Unvote this post asynchronously.
        /// Please note that votes must be cast by humans.  Automated bot-voting violates Reddit's rules.
        /// That is, API clients proxying a human's action one-for-one are OK, but bots deciding how to vote on content or amplifying a human's vote are not.
        /// See the Reddit rules for more details on what constitutes vote cheating.
        /// </summary>
        public async Task UnvoteAsync()
        {
            await Dispatch.LinksAndComments.VoteAsync(new LinksAndCommentsVoteInput(Fullname, 0));
        }

        /// <summary>
        /// Approve this post.
        /// If the thing was removed, it will be re-inserted into appropriate listings.
        /// Any reports on the approved thing will be discarded.
        /// </summary>
        public void Approve()
        {
            Dispatch.Moderation.Approve(Fullname);
        }

        /// <summary>
        /// Return information about a users's flair options.
        /// </summary>
        /// <param name="username">A valid Reddit username</param>
        /// <returns>Flair results.</returns>
        public FlairSelectorResultContainer FlairSelector(string username)
        {
            return Validate(Dispatch.Flair.FlairSelector(new FlairLinkInput(Fullname, username), Subreddit));
        }
    }
}
