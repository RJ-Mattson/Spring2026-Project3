namespace Spring2026_Project3_RJmattson.Models.ViewModels
{
    public class ViewActor
    {
        public Actor Actor { get; set; }
        public List<Movie> Movies { get; set; }
        public List<ViewAI> AITweets { get; set; }
        public double AverageSentiment { get; set; }
    }
}
