namespace Spring2026_Project3_RJmattson.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string gender { get; set; }
        public int Age { get; set; }
        public string Imbdlink { get; set; }
        public byte[]? Poster { get; set; }


        public virtual ICollection<MovieActorRel> ActorMovies { get; set; } = new List<MovieActorRel>();


    }
}
