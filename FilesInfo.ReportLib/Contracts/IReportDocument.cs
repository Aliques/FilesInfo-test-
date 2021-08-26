

namespace FilesInfo.ReportLib.Contracts
{
    public interface IReportDocument
    {
        FileType FileType { get; }
        string BuildDocument();
    }
    public enum FileType
    {
        Html,
        Json,
        Xml,
        Csv
        //и.т.д
    }
}
