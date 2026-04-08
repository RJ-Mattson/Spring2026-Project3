namespace Spring2026_Project3_RJmattson.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int ReleaseYear { get; set; }
        public byte[]? Poster { get; set; }
        public string Imbdlink { get; set; }
        public virtual ICollection<MovieActorRel> Movies { get; set; } = new List<MovieActorRel>();


    }
}
