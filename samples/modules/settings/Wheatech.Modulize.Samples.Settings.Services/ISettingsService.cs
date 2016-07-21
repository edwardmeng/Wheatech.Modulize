namespace Wheatech.Modulize.Samples.Settings.Services
{
    public interface ISettingsService
    {
        string Get(string key);

        void Set(string key, string value);
    }
}
