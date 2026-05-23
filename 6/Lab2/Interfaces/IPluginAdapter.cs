namespace Lab2.Interfaces
{
    /// <summary>
    /// Adapter pattern interface for adapting foreign plugins
    /// </summary>
    public interface IPluginAdapter
    {
        string AdaptData(string data);
        string ReverseAdapt(string data);
        string GetSourceFormat();
        string GetTargetFormat();
    }
}