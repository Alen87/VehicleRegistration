namespace Project.DAL.Entities;


public class VehicleEngineType
{

    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;

    /// <summary>
    /// Navigacijsko svojstvo - veza više-na-više s modelima vozila kroz spojnu tablicu
    /// </summary>
    public virtual ICollection<VehicleModelEngineType> ModelEngineTypes { get; set; } = new List<VehicleModelEngineType>();
}