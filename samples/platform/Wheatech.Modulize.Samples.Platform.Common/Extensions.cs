namespace Wheatech.Modulize.Samples.Platform.Common
{
    public static class Extensions
    {
        public static TService GetService<TService>(this object handler)
        {
            return (TService)Startup.Environment.Get(typeof(TService));
        }
    }
}