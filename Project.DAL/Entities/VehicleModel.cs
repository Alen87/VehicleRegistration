namespace Project.DAL.Entities;


public class VehicleModel
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;
    public int MakeId { get; set; }

    public virtual VehicleMake Make { get; set; } = null!;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovaj model
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();

    /// <summary>
    /// Navigacijsko svojstvo - veza više-na-više s tipovima motora kroz spojnu tablicu
    /// </summary>
    public virtual ICollection<VehicleModelEngineType> ModelEngineTypes { get; set; } = new List<VehicleModelEngineType>();
}