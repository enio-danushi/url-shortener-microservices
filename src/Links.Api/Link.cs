namespace Links.Api;

public class Link
{
  public required string Code { get; set;}
  public required string Url { get; set;}
  public DateTimeOffset CreatedAt { get; set;}
  public DateTimeOffset? DisabledAt { get; set;}  
}