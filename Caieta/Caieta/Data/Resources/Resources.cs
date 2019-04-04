namespace Caieta
{
    public static class Resources
    {   
        // Shortcut to grab Content Manager resources
        public static T Get<T>(string filename)
        {
            return Engine.Instance.Content.Load<T>(filename);
        }
    }
}
