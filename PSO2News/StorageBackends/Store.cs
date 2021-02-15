using System;

namespace PSO2News.StorageBackends
{
    public abstract class Store : IDisposable
    {
        public abstract void Save(PersistentData data);

        public abstract PersistentData Load();

        public void Dispose()
        {
            // nothing to do
        }
    }
}