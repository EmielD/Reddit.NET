﻿using Reddit.Controllers.EventArgs;
using Reddit.Controllers.Internal;
using Reddit.Controllers.Structures;
using Reddit.Exceptions;
using Reddit.Inputs.PrivateMessages;
using Reddit.Things;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reddit.Controllers
{
    /// <summary>
    /// Controller class for private messages.
    /// </summary>
    public class PrivateMessages : Monitors
    {
        public event EventHandler<MessagesUpdateEventArgs> InboxUpdated;
        public event EventHandler<MessagesUpdateEventArgs> UnreadUpdated;
        public event EventHandler<MessagesUpdateEventArgs> SentUpdated;

        internal override Models.Internal.Monitor MonitorModel => Dispatch.Monitor;
        internal override ref MonitoringSnapshot Monitoring => ref MonitorModel.Monitoring;

        /// <summary>
        /// List of inbox messages.
        /// </summary>
        public List<Message> Inbox
        {
            get
            {
                return (InboxLastUpdated.HasValue
                    && InboxLastUpdated.Value.AddSeconds(15) > DateTime.Now ? inbox : GetMessagesInbox(false));
            }
            private set
            {
                inbox = value;
            }
        }
        internal List<Message> inbox;

        /// <summary>
        /// List of unread messages.
        /// </summary>
        public List<Message> Unread
        {
            get
            {
                return (UnreadLastUpdated.HasValue
                    && UnreadLastUpdated.Value.AddSeconds(15) > DateTime.Now ? unread : GetMessagesUnread(false));
            }
            private set
            {
                unread = value;
            }
        }
        internal List<Message> unread;

        /// <summary>
        /// List of sent messages.
        /// </summary>
        public List<Message> Sent
        {
            get
            {
                return (SentLastUpdated.HasValue
                    && SentLastUpdated.Value.AddSeconds(15) > DateTime.Now ? sent : GetMessagesSent(false));
            }
            private set
            {
                sent = value;
            }
        }
        internal List<Message> sent;

        private DateTime? InboxLastUpdated;
        private DateTime? UnreadLastUpdated;
        private DateTime? SentLastUpdated;

        private Dispatch Dispatch;

        /// <summary>
        /// Create a new instance of the private messages controller.
        /// </summary>
        /// <param name="dispatch"></param>
        /// <param name="inbox"></param>
        /// <param name="unread"></param>
        /// <param name="sent"></param>
        public PrivateMessages(Dispatch dispatch, List<Message> inbox = null, List<Message> unread = null,
            List<Message> sent = null) 
            : base()
        {
            Inbox = inbox ?? new List<Message>();
            Unread = unread ?? new List<Message>();
            Sent = sent ?? new List<Message>();

            Threads = new Dictionary<string, Thread>();

            Dispatch = dispatch;
        }

        /// <summary>
        /// Retrieve private messages for the current user.
        /// </summary>
        /// <param name="where">One of (inbox, unread, sent)</param>
        /// <param name="mark">one of (true, false)</param>
        /// <param name="limit">the maximum number of items desired (default: 25, maximum: 100)</param>
        /// <param name="after">fullname of a thing</param>
        /// <param name="before">fullname of a thing</param>
        /// <param name="show">(optional) the string all</param>
        /// <param name="srDetail">(optional) expand subreddits</param>
        /// <param name="includeCategories">boolean value</param>
        /// <param name="count">a positive integer (default: 0)</param>
        /// <param name="mid"></param>
        /// <returns>A list of messages.</returns>
        public List<Message> GetMessages(string where, bool mark = true, int limit = 25, string after = "", string before = "",
            string show = "all", bool srDetail = false, bool includeCategories = false, int count = 0, string mid = "")
        {
            return GetMessages(where, new PrivateMessagesGetMessagesInput(mark, mid, after, before, includeCategories,
                count, limit, show, srDetail));
        }

        /// <summary>
        /// Retrieve private messages for the current user.
        /// </summary>
        /// <param name="where">One of (inbox, unread, sent)</param>
        /// <param name="privateMessagesGetMessagesInput">A valid PrivateMessagesGetMessagesInput instance</param>
        /// <returns>A list of messages.</returns>
        public List<Message> GetMessages(string where, PrivateMessagesGetMessagesInput privateMessagesGetMessagesInput)
        {
            MessageContainer messageContainer = Dispatch.PrivateMessages.GetMessages(where, privateMessagesGetMessagesInput);

            List<Message> res = new List<Message>();
            if (messageContainer != null && messageContainer.Data != null && messageContainer.Data.Children != null)
            {
                foreach (MessageChild messageChild in messageContainer.Data.Children)
                {
                    res.Add(messageChild.Data);
                }
            }

            switch (where)
            {
                case "inbox":
                    InboxLastUpdated = DateTime.Now;
                    Inbox = res;
                    break;
                case "unread":
                    UnreadLastUpdated = DateTime.Now;
                    Unread = res;
                    break;
                case "sent":
                    SentLastUpdated = DateTime.Now;
                    Sent = res;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Retrieve private inbox messages for the current user.
        /// </summary>
        /// <param name="mark">one of (true, false)</param>
        /// <param name="limit">the maximum number of items desired (default: 25, maximum: 100)</param>
        /// <param name="after">fullname of a thing</param>
        /// <param name="before">fullname of a thing</param>
        /// <param name="show">(optional) the string all</param>
        /// <param name="srDetail">(optional) expand subreddits</param>
        /// <param name="includeCategories">boolean value</param>
        /// <param name="count">a positive integer (default: 0)</param>
        /// <param name="mid"></param>
        /// <returns>A list of messages.</returns>
        public List<Message> GetMessagesInbox(bool mark = true, int limit = 25, string after = "", string before = "",
            string show = "all", bool srDetail = false, bool includeCategories = false, int count = 0, string mid = "")
        {
            return GetMessages("inbox", mark, limit, after, before, show, srDetail, includeCategories, count, mid);
        }

        /// <summary>
        /// Retrieve private unread messages for the current user.
        /// </summary>
        /// <param name="mark">one of (true, false)</param>
        /// <param name="limit">the maximum number of items desired (default: 25, maximum: 100)</param>
        /// <param name="after">fullname of a thing</param>
        /// <param name="before">fullname of a thing</param>
        /// <param name="show">(optional) the string all</param>
        /// <param name="srDetail">(optional) expand subreddits</param>
        /// <param name="includeCategories">boolean value</param>
        /// <param name="count">a positive integer (default: 0)</param>
        /// <param name="mid"></param>
        /// <returns>A list of messages.</returns>
        public List<Message> GetMessagesUnread(bool mark = true, int limit = 25, string after = "", string before = "",
            string show = "all", bool srDetail = false, bool includeCategories = false, int count = 0, string mid = "")
        {
            return GetMessages("unread", mark, limit, after, before, show, srDetail, includeCategories, count, mid);
        }

        /// <summary>
        /// Retrieve private sent messages for the current user.
        /// </summary>
        /// <param name="mark">one of (true, false)</param>
        /// <param name="limit">the maximum number of items desired (default: 25, maximum: 100)</param>
        /// <param name="after">fullname of a thing</param>
        /// <param name="before">fullname of a thing</param>
        /// <param name="show">(optional) the string all</param>
        /// <param name="srDetail">(optional) expand subreddits</param>
        /// <param name="includeCategories">boolean value</param>
        /// <param name="count">a positive integer (default: 0)</param>
        /// <param name="mid"></param>
        /// <returns>A list of messages.</returns>
        public List<Message> GetMessagesSent(bool mark = true, int limit = 25, string after = "", string before = "",
            string show = "all", bool srDetail = false, bool includeCategories = false, int count = 0, string mid = "")
        {
            return GetMessages("sent", mark, limit, after, before, show, srDetail, includeCategories, count, mid);
        }

        /// <summary>
        /// Queue up marking all messages for a user as read.
        /// This may take some time, and returns 202 to acknowledge acceptance of the request.
        /// </summary>
        /// <param name="filterTypes">A comma-separated list of items</param>
        public void MarkAllRead(string filterTypes = "")
        {
            Dispatch.PrivateMessages.ReadAllMessages(filterTypes);
        }

        /// <summary>
        /// Asynchronously queue up marking all messages for a user as read.
        /// This may take some time, and returns 202 to acknowledge acceptance of the request.
        /// </summary>
        /// <param name="filterTypes">A comma-separated list of items</param>
        public async Task MarkAllReadAsync(string filterTypes = "")
        {
            await Dispatch.PrivateMessages.ReadAllMessagesAsync(filterTypes);
        }

        /// <summary>
        /// Collapse a message.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public void CollapseMessage(string ids)
        {
            Dispatch.PrivateMessages.CollapseMessage(ids);
        }

        /// <summary>
        /// Collapse a message asynchronously.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public async Task CollapseMessageAsync(string ids)
        {
            await Dispatch.PrivateMessages.CollapseMessageAsync(ids);
        }

        /// <summary>
        /// Delete messages from the recipient's view of their inbox.
        /// </summary>
        /// <param name="id">fullname of a thing</param>
        public void DeleteMessage(string id)
        {
            Dispatch.PrivateMessages.DelMsg(id);
        }

        /// <summary>
        /// Delete messages from the recipient's view of their inbox asynchronously.
        /// </summary>
        /// <param name="id">fullname of a thing</param>
        public async Task DeleteMessageAsync(string id)
        {
            await Dispatch.PrivateMessages.DelMsgAsync(id);
        }

        /// <summary>
        /// Mark a message as read.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public void ReadMessage(string ids)
        {
            Dispatch.PrivateMessages.ReadMessage(ids);
        }

        /// <summary>
        /// Mark a message as read asynchronously.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public async Task ReadMessageAsync(string ids)
        {
            await Dispatch.PrivateMessages.ReadMessageAsync(ids);
        }

        /// <summary>
        /// Uncollapse a message.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public void UncollapseMessage(string ids)
        {
            Dispatch.PrivateMessages.UncollapseMessage(ids);
        }

        /// <summary>
        /// Uncollapse a message asynchronously.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public async Task UncollapseMessageAsync(string ids)
        {
            await Dispatch.PrivateMessages.UncollapseMessageAsync(ids);
        }

        /// <summary>
        /// Mark a message as unread.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public void UnreadMessage(string ids)
        {
            Dispatch.PrivateMessages.UnreadMessage(ids);
        }

        /// <summary>
        /// Mark a message as unread asynchronously.
        /// </summary>
        /// <param name="ids">A comma-separated list of thing fullnames</param>
        public async Task UnreadMessageAsync(string ids)
        {
            await Dispatch.PrivateMessages.UnreadMessageAsync(ids);
        }

        /// <summary>
        /// Send a private message.
        /// </summary>
        /// <param name="to">the name of an existing user</param>
        /// <param name="subject">a string no longer than 100 characters</param>
        /// <param name="text">raw markdown text</param>
        /// <param name="fromSr">subreddit name</param>
        /// <param name="gRecaptchaResponse"></param>
        public void Compose(string to, string subject, string text, string fromSr = "", string gRecaptchaResponse = "")
        {
            Validate(Dispatch.PrivateMessages.Compose(new PrivateMessagesComposeInput(fromSr, subject, text, to), gRecaptchaResponse));
        }

        /// <summary>
        /// Send a private message asynchronously.
        /// </summary>
        /// <param name="to">the name of an existing user</param>
        /// <param name="subject">a string no longer than 100 characters</param>
        /// <param name="text">raw markdown text</param>
        /// <param name="fromSr">subreddit name</param>
        /// <param name="gRecaptchaResponse"></param>
        public async Task ComposeAsync(string to, string subject, string text, string fromSr = "", string gRecaptchaResponse = "")
        {
            Validate(await Dispatch.PrivateMessages.ComposeAsync(new PrivateMessagesComposeInput(fromSr, subject, text, to), gRecaptchaResponse));
        }

        /// <summary>
        /// Send a private message.
        /// </summary>
        /// <param name="privateMessagesComposeInput">A valid PrivateMessagesComposeInput instance</param>
        /// <param name="gRecaptchaResponse"></param>
        public void Compose(PrivateMessagesComposeInput privateMessagesComposeInput, string gRecaptchaResponse = "")
        {
            Validate(Dispatch.PrivateMessages.Compose(privateMessagesComposeInput, gRecaptchaResponse));
        }

        /// <summary>
        /// Send a private message asynchronously.
        /// </summary>
        /// <param name="privateMessagesComposeInput">A valid PrivateMessagesComposeInput instance</param>
        /// <param name="gRecaptchaResponse"></param>
        public async Task ComposeAsync(PrivateMessagesComposeInput privateMessagesComposeInput, string gRecaptchaResponse = "")
        {
            Validate(await Dispatch.PrivateMessages.ComposeAsync(privateMessagesComposeInput, gRecaptchaResponse));
        }

        protected virtual void OnInboxUpdated(MessagesUpdateEventArgs e)
        {
            InboxUpdated?.Invoke(this, e);
        }

        protected virtual void OnUnreadUpdated(MessagesUpdateEventArgs e)
        {
            UnreadUpdated?.Invoke(this, e);
        }

        protected virtual void OnSentUpdated(MessagesUpdateEventArgs e)
        {
            SentUpdated?.Invoke(this, e);
        }

        /// <summary>
        /// Monitor inbox messages.
        /// </summary>
        /// <returns>Whether monitoring was successfully initiated.</returns>
        public bool MonitorInbox()
        {
            string key = "PrivateMessagesInbox";
            return Monitor(key, new Thread(() => MonitorInboxThread(key)), "PrivateMessages");
        }

        private void MonitorInboxThread(string key)
        {
            MonitorPrivateMessagesThread(key, "inbox");
        }

        /// <summary>
        /// Monitor unread messages.
        /// </summary>
        /// <returns>Whether monitoring was successfully initiated.</returns>
        public bool MonitorUnread()
        {
            string key = "PrivateMessagesUnread";
            return Monitor(key, new Thread(() => MonitorUnreadThread(key)), "PrivateMessages");
        }

        private void MonitorUnreadThread(string key)
        {
            MonitorPrivateMessagesThread(key, "unread");
        }

        /// <summary>
        /// Monitor sent messages.
        /// </summary>
        /// <returns>Whether monitoring was successfully initiated.</returns>
        public bool MonitorSent()
        {
            string key = "PrivateMessagesSent";
            return Monitor(key, new Thread(() => MonitorSentThread(key)), "PrivateMessages");
        }

        private void MonitorSentThread(string key)
        {
            MonitorPrivateMessagesThread(key, "sent");
        }

        private void MonitorPrivateMessagesThread(string key, string type, int startDelayMs = 0)
        {
            if (startDelayMs > 0)
            {
                Thread.Sleep(startDelayMs);
            }

            while (!Terminate
                && Monitoring.Get(key).Contains("PrivateMessages"))
            {
                List<Message> oldList;
                List<Message> newList;
                switch (type)
                {
                    default:
                        throw new RedditControllerException("Unrecognized type '" + type + "'.");
                    case "inbox":
                        oldList = inbox;
                        newList = GetMessagesInbox();
                        break;
                    case "unread":
                        oldList = unread;
                        newList = GetMessagesUnread();
                        break;
                    case "sent":
                        oldList = sent;
                        newList = GetMessagesSent();
                        break;
                }

                if (Lists.ListDiff(oldList, newList, out List<Message> added, out List<Message> removed))
                {
                    // Event handler to alert the calling app that the list has changed.  --Kris
                    MessagesUpdateEventArgs args = new MessagesUpdateEventArgs
                    {
                        NewMessages = newList,
                        OldMessages = oldList,
                        Added = added,
                        Removed = removed
                    };
                    TriggerUpdate(args, type);
                }

                Thread.Sleep(Monitoring.Count() * MonitoringWaitDelayMS);
            }
        }

        protected void TriggerUpdate(MessagesUpdateEventArgs args, string type)
        {
            switch (type)
            {
                case "inbox":
                    OnInboxUpdated(args);
                    break;
                case "unread":
                    OnUnreadUpdated(args);
                    break;
                case "sent":
                    OnSentUpdated(args);
                    break;
            }
        }

        protected override Thread CreateMonitoringThread(string key, string subKey, int startDelayMs = 0)
        {
            switch (key)
            {
                default:
                    throw new RedditControllerException("Unrecognized key.");
                case "PrivateMessagesInbox":
                    return new Thread(() => MonitorPrivateMessagesThread(key, "inbox", startDelayMs));
                case "PrivateMessagesUnread":
                    return new Thread(() => MonitorPrivateMessagesThread(key, "unread", startDelayMs));
                case "PrivateMessagesSent":
                    return new Thread(() => MonitorPrivateMessagesThread(key, "sent", startDelayMs));
            }
        }
    }
}
