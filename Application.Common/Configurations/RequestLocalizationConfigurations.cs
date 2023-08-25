namespace Application.Common.Configurations;

public class RequestLocalizationConfigurations
{
    public string DefaultRequestCulture { get; set; }

    public HashSet<string> SupportedCultures { get; set; }
}
