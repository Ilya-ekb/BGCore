using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BGCore.Samples.VContainer
{
    public sealed class LoopSystemInstaller : MonoBehaviour, IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LoopSystemVContainerAdapter>(Lifetime.Singleton);
        }
    }
}
