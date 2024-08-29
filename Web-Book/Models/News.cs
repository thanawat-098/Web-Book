namespace Web_Book.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string Details { get; set; }
        public string PrimaryImage { get; set; }
        public List<string> AdditionalImages { get; set; }

    }
}
