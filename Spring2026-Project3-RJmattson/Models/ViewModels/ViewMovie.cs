namespace Spring2026_Project3_RJmattson.Models.ViewModels
{
    public class ViewMovie
    {
        public Movie Movie { get; set; }
        public List<Actor> Actors { get; set; }
        public List<ViewAI> AIReviews { get; set; }
        public double AverageSentiment { get; set; }

    }
}
