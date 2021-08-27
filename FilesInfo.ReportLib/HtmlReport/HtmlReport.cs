
using FilesInfo.ReportLib.Contracts;
using FilesInfo.ReportLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace FilesInfo.ReportLib.HtmlReport
{
    public class HtmlReport : IReportDocument
    {
        #region Fieds
        string _path;
        private List<FilesStatisticModel> extentionList;
        private XElement commonInformationTableRows;
        #endregion
        public HtmlReport(string path)
        {
            _path = path;
            FileType = FileType.Html;
        }
        #region Properties
        public FileType FileType { get; }
        #endregion

        #region Methods
        public string BuildDocument()
        {
           var mainTable = CommonInformationTableRowsBuilder();
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
                            new XElement("table", new XAttribute("class", "table table-sm"),
                                new XElement("thead", new XAttribute("class", "thead-dark"),
                                    new XElement("tr",
                                        new XElement("th", "№", new XAttribute("scope", "col")),
                                        new XElement("th", "MimeType", new XAttribute("scope", "col")),
                                        new XElement("th", "Колличество", new XAttribute("scope", "col")),
                                        new XElement("th", "Доля (%)", new XAttribute("scope", "col")),
                                        new XElement("th", "Сдедний размер (кБайт)", new XAttribute("scope", "col"))
                                    )),
                        ExtentionsStatisticsBuilder()
                        ))),
                        new XElement("br", new XAttribute("class", "row")),
                         new XElement("br", new XAttribute("class", "row")),
                        new XElement("div", new XAttribute("class", "row"),
                         new XElement("div", new XAttribute("class", "col-md-8"),
                            new XElement("table", new XAttribute("class", "table table-sm"),
                                new XElement("thead", new XAttribute("class", "thead-dark"),
                                    new XElement("tr",
                                        new XElement("th", "№", new XAttribute("scope", "col")),
                                        new XElement("th", "Название файла", new XAttribute("scope", "col")),
                                        new XElement("th", "Расширение/Тип", new XAttribute("scope", "col")),
                                        new XElement("th", "Размер(байты)", new XAttribute("scope", "col")),
                                        new XElement("th", "MimeType", new XAttribute("scope", "col"))
                                )),
                        mainTable
                        )))))
             );
            extentionList = null;
            return html.ToString();
        }
        /// <summary>
        /// Полный отчет
        /// </summary>
        public XElement CommonInformationTableRowsBuilder()
        {
            commonInformationTableRows = new XElement("tbody");
            extentionList = new List<FilesStatisticModel>();
            string[] files;
            int rowNumber = 0;
            try
            {
                files = Directory.GetFileSystemEntries(_path, "*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Нет доступа к системным файлам");// System.Windows.MessageBox.Show("Нет доступа к системным файлам");
                //return null;
            }
            foreach (var item in files)
            {
                var mimeType = System.Web.MimeMapping.GetMimeMapping(item);
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    var dirInfo = new DirectoryInfo(item);
                    var size = SafeEnumerateFiles(item, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
                    commonInformationTableRows.Add(new XElement("tr",
                                   new XElement("th", new XAttribute("scope", "row"), rowNumber++),
                                   new XElement("td", dirInfo.Name),
                                   new XElement("td", "Folder"),
                                   new XElement("td", size),
                                   new XElement("td", mimeType)
                                   ));
                    var existItem = extentionList.Find(o => o.MimeType == mimeType);
                    if (existItem == null)
                    {
                        extentionList.Add(new FilesStatisticModel
                        {
                            ExtentionName = dirInfo.Name,
                            MimeType = mimeType,
                            Size = size,
                            TotalSize = size,
                            TotalCount = 1
                        });
                    }
                    else
                    {
                        existItem.TotalCount += 1;
                        existItem.TotalSize += size;
                    }
                }
                else
                {
                    var fileInfo = new FileInfo(item);
                    var size = Math.Round((double)fileInfo.Length / 1024, 2);
                    commonInformationTableRows.Add(new XElement("tr",
                                   new XElement("th", new XAttribute("scope", "row"), rowNumber++),
                                   new XElement("td", fileInfo.Extension == string.Empty ? "Неизвестный формат" : fileInfo.Extension),
                                   new XElement("td", fileInfo.Name),
                                   new XElement("td", size),
                                   new XElement("td", mimeType)
                                   ));
                    var existItem = extentionList.Find(o => o.MimeType == mimeType);
                    if (existItem == null)
                    {
                        extentionList.Add(new FilesStatisticModel
                        {
                            ExtentionName = fileInfo.Extension,
                            MimeType = mimeType,
                            Size = size,
                            TotalSize = size,
                            TotalCount = 1
                        });
                    }
                    else
                    {
                        existItem.TotalCount += 1;
                        existItem.TotalSize += size; 
                    }
                }

            }
            return commonInformationTableRows;
        }

        /// <summary>
        /// Статистика
        /// </summary>
        public XElement ExtentionsStatisticsBuilder()
        {
            var tbody = new XElement("tbody");
            var s = extentionList;
            
            var distinctedList = extentionList.Distinct();
            var countItems = (double)distinctedList.Count();
            var totalCount = distinctedList.Sum(o => o.TotalCount);
            var multiplier = countItems * 0.1;
            int rowNumber = 0;

            foreach (var val in distinctedList)
            {
                var extentionCount = distinctedList.Where(x => x == val).Count();
                tbody.Add(new XElement("tr",
                            new XElement("th", new XAttribute("scope", "row"), rowNumber++),
                           new XElement("td", val.MimeType),
                           new XElement("td", val.TotalCount),
                           new XElement("td", Math.Round(val.TotalCount / (double)totalCount*100,5)),
                           new XElement("td", Math.Round(val.TotalSize/val.TotalCount,3))
                ));
            }
            return tbody;
        }
        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDirPath = dirs.Pop();
                if (searchOption == SearchOption.AllDirectories)
                {
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(currentDirPath);
                        foreach (string subDirPath in subDirs)
                        {
                            dirs.Push(subDirPath);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string filePath in files)
                {
                    yield return filePath;
                }
            }
        }
        #endregion
    }
}
