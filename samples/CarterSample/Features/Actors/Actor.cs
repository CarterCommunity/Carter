namespace CarterSample.Features.Actors
{
    public class Actor
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Age { get; set; }
        
        public ActorStatus Status { get; set; }
    }
    
    public enum ActorStatus
    {
        Active,
        Retired,
        Deceased
    }
}