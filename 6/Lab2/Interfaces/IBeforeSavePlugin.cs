namespace Lab2
{
    public interface IBeforeSavePlugin
    {
        string GetName();
        string ProcessBeforeSave(string data);
        bool IsEnabled { get; set; }
        void ShowSettings();
    }
}