﻿using Newtonsoft.Json;
using Reddit.NET.Models.Structures;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reddit.NET.Models
{
    public class Emoji : BaseModel
    {
        internal override RestClient RestClient { get; set; }

        public Emoji(string appId, string refreshToken, string accessToken, RestClient restClient) : base(appId, refreshToken, accessToken, restClient) { }

        // TODO - Needs testing.
        /// <summary>
        /// Add an emoji to the DB by posting a message on emoji_upload_q.
        /// A job processor that listens on a queue, uses the s3_key provided in the request to locate the image in S3 Temp Bucket and moves it to the PERM bucket.
        /// It also adds it to the DB using name as the column and sr_fullname as the key and sends the status on the websocket URL that is provided as part of this response.
        /// </summary>
        /// <param name="subreddit">The subreddit with the emojis</param>
        /// <param name="name">Name of the emoji to be created. It can be alphanumeric without any special characters except '-' & '_' and cannot exceed 24 characters</param>
        /// <param name="s3Key">S3 key of the uploaded image which can be obtained from the S3 url. This is of the form subreddit/hash_value</param>
        /// <returns>(TODO - Untested)</returns>
        public object Add(string subreddit, string name, string s3Key)
        {
            RestRequest restRequest = PrepareRequest("api/v1/" + subreddit + "/emoji.json", Method.POST);

            restRequest.AddParameter("name", name);
            restRequest.AddParameter("s3_key", s3Key);

            return JsonConvert.DeserializeObject(ExecuteRequest(restRequest));
        }

        // TODO - Needs testing.
        /// <summary>
        /// Delete a Subreddit emoji. Remove the emoji from Cassandra and purge the assets from S3 and the image resizing provider.
        /// </summary>
        /// <param name="subreddit">The subreddit with the emojis</param>
        /// <param name="emojiName">The name of the emoji to be deleted</param>
        /// <returns>(TODO - Untested)</returns>
        public object Delete(string subreddit, string emojiName)
        {
            return JsonConvert.DeserializeObject(ExecuteRequest("api/v1/" + subreddit + "/emoji/" + emojiName, Method.DELETE));
        }

        // TODO - Needs testing.
        /// <summary>
        /// Acquire and return an upload lease to s3 temp bucket.
        /// The return value of this function is a json object containing credentials for uploading assets to S3 bucket, S3 url for upload request and the key to use for uploading.
        /// Using this lease the client will upload the emoji image to S3 temp bucket (included as part of the S3 URL). This lease is used by S3 to verify that the upload is authorized.
        /// </summary>
        /// <param name="subreddit">The subreddit with the emojis</param>
        /// <param name="filePath">name and extension of the image file e.g. image1.png</param>
        /// <param name="mimeType">mime type of the image e.g. image/png</param>
        /// <returns>(TODO - Untested)</returns>
        public object AcquireLease(string subreddit, string filePath, string mimeType)
        {
            RestRequest restRequest = PrepareRequest("api/v1/" + subreddit + "/emoji_asset_upload_s3.json", Method.POST);

            restRequest.AddParameter("filepath", filePath);
            restRequest.AddParameter("mimetype", mimeType);

            return JsonConvert.DeserializeObject(ExecuteRequest(restRequest));
        }

        // TODO - Needs testing.
        /// <summary>
        /// Set custom emoji size. Omitting width or height will disable custom emoji sizing.
        /// </summary>
        /// <param name="subreddit">The subreddit with the emojis</param>
        /// <param name="height">an integer between 1 and 40 (default: 0)</param>
        /// <param name="width">an integer between 1 and 40 (default: 0)</param>
        /// <returns>(TODO - Untested)</returns>
        public object CustomSize(string subreddit, int height = 0, int width = 0)
        {
            RestRequest restRequest = PrepareRequest("api/v1/" + subreddit + "/emoji_custom_size", Method.POST);

            restRequest.AddParameter("height", height);
            restRequest.AddParameter("width", width);

            return JsonConvert.DeserializeObject(ExecuteRequest(restRequest));
        }

        /// <summary>
        /// Get all emojis for a SR. The response includes reddit emojis as well as emojis for the SR specified in the request.
        /// </summary>
        /// <param name="subreddit">The subreddit with the emojis</param>
        /// <returns>Emojis.</returns>
        public object All(string subreddit)
        {
            return JsonConvert.DeserializeObject(ExecuteRequest("api/v1/" + subreddit + "/emojis/all"));
        }
    }
}