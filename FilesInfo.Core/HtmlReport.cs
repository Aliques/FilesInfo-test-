using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FilesInfo.Core
{
    public class HtmlReport<T> : IBuildDocument
        where T : IEnumerable
    {
        private IEnumerable<string> data;
        private List<string> extentionList;

        public HtmlReport(T data)
        {
            this.data = data as IEnumerable<string>;
        }
        public string BuildDocument()
        {
            var mainTable = RowBuilder();
            XElement html = new XElement("html",
                  new XAttribute("lang", "en"),
              new XElement("head",
                  new XElement("meta", new XAttribute("charset", "utf-8")),
                  new XElement("link",
                      new XAttribute("rel", "stylesheet"),
                      new XAttribute("href", "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.4/css/bootstrap.min.css"),
                      new XAttribute("integrity", "2hfp1SzUoho7/TsGGGDaFdsuuDL0LX2hnUp6VkX3CUQ2K4K+xjboZdsXyp4oUHZj"),
                      new XAttribute("crossorigin", "anonymous")),
                  new XElement("script", String.Empty,
                      new XAttribute("src", "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.4/js/bootstrap.min.js"),
                      new XAttribute("integrity", "VjEeINv9OSwtWFLAtmc4JCtEJXXBub00gtSnszmspDLCtC0I4z4nqz7rEFbIZLLU"),
                      new XAttribute("crossorigin", "anonymous"))),
              new XElement("body",
                    new XElement("div", new XAttribute("class", "container-fluid"),
                        new XElement("div", new XAttribute("class", "row"),
                        new XElement("div", new XAttribute("class", "col col-lg-3"),
                            new XElement("table",new XAttribute("class", "table table-sm"),
                                new XElement("thead",new XAttribute("class","thead-dark"),
                                    new XElement("tr",
                                        new XElement("th","Тип файла", new XAttribute("scope","col")),
                                        new XElement("th","Колличество", new XAttribute("scope","col"))
                                    )),
                        ExtentionsStatisticsBuilder(extentionList)
                        ))),
                        new XElement("div", new XAttribute("class","row"),
                         new XElement("div", new XAttribute("class", "col-md-8"),
                            new XElement("table", new XAttribute("class", "table table-sm"),
                                new XElement("thead", new XAttribute("class", "thead-dark"),
                                    new XElement("tr",
                                        new XElement("th", "№", new XAttribute("scope", "col")),
                                        new XElement("th", "Название файла", new XAttribute("scope", "col")),
                                        new XElement("th", "Расширение", new XAttribute("scope", "col")),
                                        new XElement("th", "Размер(байты)", new XAttribute("scope", "col")),
                                        new XElement("th", "Путь", new XAttribute("scope", "col"))
                                )),
                        mainTable)))))
             );
            return html.ToString();
        }
      
        /// <summary>
        /// Extention statistics table builder
        /// </summary>
        /// <param name="extentions">Extentions list</param>
        public XElement ExtentionsStatisticsBuilder(IEnumerable<string> extentions)
        {
            var tbody = new XElement("tbody");

            foreach (var val in extentions.Distinct())
            {
                    tbody.Add(new XElement("tr",
                               new XElement("td", val==string.Empty?"Неизвестный формат":val),
                               new XElement("td", extentions.Where(x => x == val).Count())
                               ));
            }
            return tbody;
        }

        /// <summary>
        /// Html table row builder
        /// </summary>
        /// <returns>Returns a table body</returns>
        public XElement RowBuilder()
        {
            var tbody = new XElement("tbody");
            extentionList = new List<string>();
            int rowNumber = 0;
            foreach (var item in data)
            {
                var fileInfo = new FileInfo(item);
                tbody.Add(new XElement("tr",
                               new XElement("th", new XAttribute("scope", "row"), rowNumber++),
                               new XElement("td", fileInfo.Name),
                               new XElement("td",fileInfo.Extension==string.Empty? "Неизвестный формат": fileInfo.Extension),
                               new XElement("td", fileInfo.Length),
                               new XElement("td", fileInfo.DirectoryName)
                               ));
                extentionList.Add(fileInfo.Extension);
            }
            return tbody;
        }
    }
}
