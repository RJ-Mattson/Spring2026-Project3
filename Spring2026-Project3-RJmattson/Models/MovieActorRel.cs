namespace Spring2026_Project3_RJmattson.Models
{
    public class MovieActorRel
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public virtual Actor? Actor { get; set; }
        public int MovieId { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
