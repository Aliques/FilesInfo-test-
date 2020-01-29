using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesInfo.Core
{
    public class HtmlDocumentService : IDocumentSettings
    {
        public string DefaultFileName => "HtmlDocument";

        public string DefaultExtention => ".html";

        public string DefaultFilter => "Html documents (.html)|*.html";
    }
}
