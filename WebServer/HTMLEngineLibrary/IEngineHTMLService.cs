namespace HTMLEngineLibrary
{
    public interface IEngineHTMLService
    {
        string GetHTML(string template, object model);
        string GetHTML(Stream pathTemplate, object model);
        string GetHTML(byte[] bytes, object model);
        Stream GetHTMLInStream(string template, object model);
        Stream GetHTMLInStream(Stream pathTemplate, object model);
        Stream GetHTMLInStream(byte[] bytes, object model);
        byte[] GetHTMLInByte(string template, object model);
        byte[] GetHTMLInByte(Stream pathTemplate, object model);
        byte[] GetHTMLInByte(byte[] bytes, object model);
        void GenerateAndSaveInDirectory(string template, string outputPath,
            string outputFileName, object model);
        void GenerateAndSaveInDirectory(Stream templatePath, string outputPath,
            string outputFileName, object model);
        void GenerateAndSaveInDirectory(byte[] bytes, string outputPath,
            string outputFileName, object model);
        Task GenerateAndSaveTask(string template, string outputPath,
            string outputFileName, object model);
        Task GenerateAndSaveTask(Stream templatePath, string outputPath,
            string outputFileName, object model);
        Task GenerateAndSaveTask(byte[] bytes, string outputPath,
            string outputFileName, object model);

    }
}