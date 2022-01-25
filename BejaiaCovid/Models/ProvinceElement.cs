namespace BejaiaCovid.Models;

public class ProvinceElement
{
    public LatestProvinceElement[] Data { get; set; }
    public Double Latitude { get; set; }
    public Double Longitude { get; set; }
    public string Name { get; set; }
    public int ProvinceId { get; set; }
    public DateTime FirstReported { get; set; }
    public DateTime LastReported { get; set; }
}