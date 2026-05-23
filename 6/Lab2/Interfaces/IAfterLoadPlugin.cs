namespace Lab2
{
    public interface IAfterLoadPlugin
    {
        string GetName();
        string ProcessAfterLoad(string data);
        bool IsEnabled { get; set; }
        void ShowSettings();
    }
}