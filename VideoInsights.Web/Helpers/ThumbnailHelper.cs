using VideoInsights.Models;

namespace VideoInsights.Web.Helpers
{
    public static class ThumbnailHelper
    {
        public static string AddData(ThumbnailData thumbnail)
        {
            return $"data:image/png;base64,{thumbnail.Content}";
        }
    }
}
