namespace Tutorial8.Models.DTOs;

public class ClientsDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Pesel { get; set; }
}

public class ClientsTrips
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public List<TripDTO> Trips { get; set; }
}