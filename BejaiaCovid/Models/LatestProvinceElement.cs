namespace BejaiaCovid.Models;

public class LatestProvinceElement
{
    public DateTime Date { get; set; }
    public int ProvinceId { get; set; }
    public int Confirmed { get; set; }
    public int Recovered { get; set; }
    public int Deaths { get; set; }
    public int NewConfirmed { get; set; }
    public int NewRecovered { get; set; }
    public int NewDeaths { get; set; }
    public double Avg7Confirmed { get; set; }
    public double Avg7Recovered { get; set; }
    public double Avg7Deaths { get; set; }
}