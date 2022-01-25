namespace BejaiaCovid.Models;

public class LatestElement
{
    public DateTime Date { get; set; }
    public int Confirmed { get; set; }
    public int Recovered { get; set; }
    public int Deaths { get; set; }
    public int Treatment { get; set; }
    public int NewConfirmed { get; set; }
    public int NewRecovered { get; set; }
    public int NewDeaths { get; set; }
    public int NewTreatment { get; set; }
    public double Avg7Confirmed { get; set; }
    public double Avg7Recovered { get; set; }
    public double Avg7Deaths { get; set; }
    public double Avg7Treatment { get; set; }
    public DateTime UpdatedAt { get; set; }
}