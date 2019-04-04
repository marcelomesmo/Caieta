using System;

namespace Caieta.Data
{
    public static class ResourceManager
    {
        public static abstract T Load<T>(ResourceManager instance, string filename);
    }
}
