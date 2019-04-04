using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Caieta.Data
{
    class MyContentManager : ContentManager
    {
        public MyContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        Dictionary<string, object> loadedAssets = new Dictionary<string, object>();
        List<IDisposable> disposableAssets = new List<IDisposable>();


        public override T Load<T>(string assetName)
        {
            if (loadedAssets.ContainsKey(assetName))
                return (T)loadedAssets[assetName];

            T asset = ReadAsset<T>(assetName, RecordDisposableAsset);

            loadedAssets.Add(assetName, asset);

            return asset;
        }

        public override void Unload()
        {
            foreach (IDisposable disposable in disposableAssets)
                disposable.Dispose();

            loadedAssets.Clear();
            disposableAssets.Clear();
        }


        void RecordDisposableAsset(IDisposable disposable)
        {
            disposableAssets.Add(disposable);
        }
    }
}
