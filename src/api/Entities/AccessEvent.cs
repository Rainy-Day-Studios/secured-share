namespace Entities;

public class AccessEvent
{
    public string Id { get; set; }
    public string RelatedRecordId { get; set; }
    
    public DateTime EventDate { get; set; }
}