using System.Globalization;

namespace CommandQuery.Sample.AspNet.WebApi.Handlers
{
    public interface ICultureService
    {
        void SetCurrentCulture(string name);
    }

    public class CultureService : ICultureService
    {
        public void SetCurrentCulture(string name)
        {
            var culture = CultureInfo.CreateSpecificCulture(name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}