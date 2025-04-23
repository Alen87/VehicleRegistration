namespace Project.DAL.Entities;


public class VehicleOwner
{

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DOB { get; set; }

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovog vlasnika
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();
}