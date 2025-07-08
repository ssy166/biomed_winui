using System;

namespace biomed.Models
{
    public class ResearchPaper
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Journal { get; set; }
        public DateTime PublicationDate { get; set; }
        public string PublicationYear => PublicationDate.Year.ToString();
        public string Doi { get; set; }
        public string AbstractText { get; set; }
    }
} 