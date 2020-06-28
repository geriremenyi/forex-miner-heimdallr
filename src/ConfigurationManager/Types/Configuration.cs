namespace ForexMiner.Heimdallr.ConfigurationManager.Types
{
    using System;

    public class Configuration
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime LastModified { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
