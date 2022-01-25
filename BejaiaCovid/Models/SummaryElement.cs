namespace BejaiaCovid.Models;

public class SummaryElement
{
    public LatestElement Latest { get; set; }
    public AgeElement[] Age { get; set; }
    public GenderElement Gender { get; set; }
    public int Days { get; set; }
    public RatesElement Rates { get; set; }
}