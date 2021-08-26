using FilesInfo.ReportLib.Contracts;


namespace FilesInfo.ReportLib.HtmlReport
{
    public class HtmlDocumentSettings : IDocumentSettings
    {
        public string DefaultFileName => "HtmlDocument";

        public string DefaultExtention => ".html";

        public string DefaultFilter => "Html documents (.html)|*.html";
    }
}
