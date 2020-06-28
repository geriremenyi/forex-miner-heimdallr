namespace ForexMiner.Heimdallr.ConfigurationManager.Data
{
    using System;

    public class Secret
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime LastModified { get; set; }
    }
}
