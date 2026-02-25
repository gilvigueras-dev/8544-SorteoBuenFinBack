namespace SAT_API.Domain.Entities.Jobs
{
    public class DatabrickSettings
    {
        public List<Jobs> Jobs { get; set; } = new ();
    }

    public class Jobs {
     public long Id { get; set; }
     public string Name { get; set; } = string.Empty;
    }
}
