using Validosik.Core.Ioc;

namespace Client.Services.Injections
{
    public static class Ioc
    {
        public static ServiceContainerManager Instance { get; } = new ServiceContainerManager();
    }
}