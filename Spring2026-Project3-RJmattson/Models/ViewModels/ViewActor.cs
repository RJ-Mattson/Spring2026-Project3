using Spring2026_Project3_RJmattson.Models;

namespace Spring2026_Project3_RJmattson.Models.ViewModels
{
    public class ViewActor
    {
        public Actor Actor { get; set; }
        public List<Movie> Movies { get; set; }
        public List<ViewAITweet> AITweets { get; set; }
        public double AverageSentiment { get; set; }
    }
    public class ViewAITweet
    {
        public string Tweet { get; set; }
        public double Sentiment { get; set; }
    }
}
