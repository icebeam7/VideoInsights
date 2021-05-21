using Microsoft.AspNetCore.Http;

namespace VideoInsights.Web.Models
{
    public class UploadViewModel
    {
        public IFormFile VideoFile { get; set; }
        public bool Result { get; set; }
    }
}
