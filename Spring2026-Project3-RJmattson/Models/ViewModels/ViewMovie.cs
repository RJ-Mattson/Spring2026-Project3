using Spring2026_Project3_RJmattson.Models;

namespace Spring2026_Project3_RJmattson.Models.ViewModels
{
    public class ViewMovie
    {
        public Movie Movie { get; set; }
        public List<Actor> Actors { get; set; }
        public List<ViewAIReview> AIReviews { get; set; }
        public double AverageSentiment { get; set; }

    }

    public class ViewAIReview
    {
        public string Review { get; set; }
        public double Sentiment { get; set; }
    }
}
