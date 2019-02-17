namespace NewsTextScraper.Api.Models
{
    public class WebPageData
    {
        public string Id { get; }

        public string Title { get; }

        public string Content { get;}

        public string Url { get;}

        public WebPageData(string title, string content, string url)
        {
            Title = title;
            Content = content;
            Url = url;
            Id = GetId(title, url);
        }

        public string GetId(string title, string url)
        {
            var secretKey = title + url;
            var salt = "I put some text here";
            using (var sha = System.Security.Cryptography.SHA1.Create())
            {
                var hash = sha.ComputeHash(System.Text.Encoding.UTF32.GetBytes(secretKey + salt));
                return System.Convert.ToBase64String(hash, 0, 15);
            }
        }
    }
}