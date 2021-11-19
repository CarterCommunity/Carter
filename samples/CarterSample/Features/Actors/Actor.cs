namespace CarterSample.Features.Actors;

using System.Text.Json.Serialization;

public class Actor
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Age { get; set; }
        
    [JsonIgnore]
    public string ExternalReference { get; set; }
        
    public ActorStatus Status { get; set; }
}
    
public enum ActorStatus
{
    Active,
    Retired,
    Deceased
}