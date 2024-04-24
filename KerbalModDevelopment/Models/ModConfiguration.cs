namespace KerbalModDevelopment.Models
{
    internal class ModConfiguration
    {
        public string Name { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Solution { get; set; } = string.Empty;

        public bool Build { get; set; } = true;

        public Dictionary<string, string> BuildProperties { get; set; } = new();

        public bool Deploy { get; set; } = true;
    }
}
