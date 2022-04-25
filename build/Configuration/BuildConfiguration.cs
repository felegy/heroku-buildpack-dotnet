namespace Build
{
    using Microsoft.Extensions.Configuration;

    public class BuildConfiguration
    {
        public BuildConfiguration(IConfiguration configuration)
        {
            configuration.Bind("Build", this);
        }

        public string DotnetVersion { get; set; } = "";

        public string Configuration { get; set; } = "";
    }
}
