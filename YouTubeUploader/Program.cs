using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

namespace YouTubeUploader
{
    /// <summary>
    /// YouTube Data API v3 sample: upload a video.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    /// </summary>
    internal class UploadVideo
    {
        private DatabaseUpdate databaseUpdate = new DatabaseUpdate();

        private YouTubeService youTubeService;

        private FileDetail currentFileDetailUpload;

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API: Upload Video");
            Console.WriteLine("==============================");

            var videosPath = ".";

            if (args.Length > 0)
            {
                videosPath = args[0];
            }

            try
            {

                new UploadVideo().Run(videosPath).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run(string videosPath)
        {
            youTubeService = await CreateYoutubeService();
            var filesDetails = LoadVideoFiles(videosPath);

            //var searchListRequest = youTubeService.Search.List("snippet");
            //searchListRequest.ChannelId = "UCrZnpqK0NnrDIklw9NEHlHw";
            //searchListRequest.MaxResults = 50;
            //searchListRequest.PublishedAfter = new DateTime(2019, 1, 1);

            //var r = searchListRequest.Execute();

            //foreach (var item in r.Items)
            //{
            //    Console.WriteLine(item.Snippet.ETag);
            //}

            foreach (var fileDetail in filesDetails)
                await Upload(fileDetail);
        }

        private async Task Upload(FileDetail fileDetail)
        {
            currentFileDetailUpload = fileDetail;

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = fileDetail.Title;
            video.Snippet.Description = "These videos are for educational purpose, to help people better understand some rules and referee decisions.\n\nDon't watch the videos here. Access www.squashvolt.com to watch and play the game. Enjoy!\n\n\nMore information about squash, go to www.psaworldtour.com";
            video.Snippet.Tags = new string[] { "squash", "rules" };
            video.Snippet.CategoryId = "17"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "private"; // or "private" or "public"
            video.Status.Embeddable = true;

            //video.Snippet.ETag = JsonConvert.SerializeObject(fileDetail);
            //video.ProjectDetails = new VideoProjectDetails();
            //video.ProjectDetails.Tags = new List<string>();
            //video.ProjectDetails.Tags.Add($"Date:{fileDetail.Year}");

            var filePath = fileDetail.Path;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youTubeService.Videos.Insert(video, "snippet,status,recordingDetails", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);

            var matchId = databaseUpdate.CreateMatch(currentFileDetailUpload);
            databaseUpdate.CreateVideo(currentFileDetailUpload, matchId, video.Id);

            Console.WriteLine("Video inserted in catalog successfully.");
        }

        private async Task<YouTubeService> CreateYoutubeService()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload},
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            return youtubeService;
        }

        private IList<FileDetail> LoadVideoFiles(string path)
        {
            var files = Directory.GetFiles(path, "*.avi");
            var filesDetails = new List<FileDetail>();

            foreach (var f in files)
            {
                var fileDetail = new FileDetail();
                var split = f.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                var playerSplit = split[1].Split("v"); 

                fileDetail.Title = split[1].Trim() + split[4];
                fileDetail.Path = f;
                fileDetail.Tournment = split[2].Trim();
                fileDetail.Year = int.Parse(split[2].Split(" ", StringSplitOptions.RemoveEmptyEntries)[0]);
                fileDetail.Player1 = playerSplit[0].Trim();
                fileDetail.Player2 = playerSplit[1].Trim();
                fileDetail.RefereeDecision = split[5] == "L" ? 0 : (split[5] == "S" ? 1 : 2);
                fileDetail.Index = int.Parse(split[4]);

                filesDetails.Add(fileDetail);
            }

            return filesDetails;
        }
    }

    public class FileDetail
    {
        public string Title { get; internal set; }
        public string Path { get; internal set; }
        public string Tournment { get; internal set; }
        public int Year { get; internal set; }
        public string Player1 { get; internal set; }
        public string Player2 { get; internal set; }
        public int RefereeDecision { get; internal set; }
        public int Index { get; internal set; }
    }


}