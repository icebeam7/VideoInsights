using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using VideoInsights.Models;

namespace VideoInsights.Api.Services
{
    public interface IVideoIndexerService
    {
        Task<bool> UploadVideo(IFormFile file);
        Task<List<VideoData>> GetVideos();
        Task<VideoDetailsData> GetVideoDetails(string id);
    }
}