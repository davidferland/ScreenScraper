namespace ScreenScraper.Model
{
    public class Product
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public System.Collections.Generic.List<string> Colors { get; set; }
        public System.Collections.Generic.List<string> Size { get; set; }

        public System.Collections.Generic.List<Image> Images { get; set; }
        public string Availability { get; set; }
    }
}