public abstract class LeadSource
{
    public int Id { get; set; }
    public string Name {get; set;}
    public DateTime CreatedAt { get; set; }
}

public class AdSource : LeadSource
{
    public int AdId { get; set; }
    public string Platform { get; set; } // Google, Facebook...
}

public class OpcSource : LeadSource
{
    public int OpcId { get; set; }
    public Opc Opc { get; set; } // navigation
}

public class WalkInSource : LeadSource
{
    // nothing extra needed
}