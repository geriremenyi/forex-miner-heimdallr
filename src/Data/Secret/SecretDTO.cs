namespace ForexMiner.Heimdallr.Data.Secret
{
    using System;

    public class SecretDTO
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime LastModified { get; set; }
    }
}
