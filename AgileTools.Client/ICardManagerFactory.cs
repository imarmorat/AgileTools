using AgileTools.Core;

namespace AgileTools.Client
{
    public interface ICardManagerFactory
    {
        ICardManagerClient CreateClient();
    }
}