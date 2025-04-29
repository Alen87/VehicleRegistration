using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;


public class VehicleOwner
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Ime vlasnika je obavezno.")]
    [MaxLength(50, ErrorMessage = "Ime vlasnika može imati najviše 50 znakova.")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Prezime vlasnika je obavezno.")]
    [MaxLength(50, ErrorMessage = "Prezime vlasnika može imati najviše 50 znakova.")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Datum rođenja je obavezan.")]
    public DateTime DOB { get; set; }

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovog vlasnika
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();
}