namespace Wheatech.Modulize
{
    public class DiscoverContext
    {
        public ModuleLocation Location { get; set; }

        public ManifestTable Manifest { get; set; }

        public string ShadowPath { get; set; }
    }
}
