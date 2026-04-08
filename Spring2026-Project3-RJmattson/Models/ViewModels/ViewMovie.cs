using Spring2026_Project3_RJmattson.Models;

namespace Spring2026_Project3_RJmattson.Models.ViewModels
{
    public class ViewMovie
    {
        public Movie Movie { get; set; }
        public List<Actor> Actors { get; set; }
        public List<ViewAIReview> AIReviews { get; set; }
        public string AverageSentiment { get; set; }

    }

    public class ViewAIReview
    {
        public string Tweet { get; set; }
        public string Sentiment { get; set; }
    }
}
