using FilesInfo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Windows.Input;
using System.Text;
using System.Diagnostics;

namespace FilesInfo.ViewModel
{
    public class WindowViewModel : BaseViewModel
    {

        private string reportText;
        private IDocumentSettings documentSettings;

        #region Fields
        public Visibility ProgressBarVisibility { get; set; } = Visibility.Hidden;
        public double WindowMinWidth { get; set; } = 750;
        public string FolderPath { get; set; }
        public bool CreateReportBtnIsEnabled { get; set; } = false;
        public Visibility FileManipulateButtonsVisibility { get; set; } = Visibility.Hidden;
        #endregion
        public WindowViewModel()
        {
            SelectFolderCommand = new RelayCommand(() => SelectFolder());
            CreateReportCommand = new RelayCommand(()=>  CreateReport());
            SaveOpenFileCommand = new RelayCommand(()=>SaveAndOpenFile());
            FolderFieldChangedCommand = new RealayParametrizedCommand( async(args)=> await FolderFieldChanged(args));
        }

        #region Methods
        private async Task FolderFieldChanged(object args)
        {
            string path;

            if (args is null)
            {
                return;
            }
            path = args.ToString();
            await Task.Run(
            () => 
            {
                CreateReportBtnIsEnabled = Directory.Exists(path);
            });
        }
        private void SelectFolder()
        {
            FileManipulateButtonsVisibility = Visibility.Hidden;
            List<string> files=new List<string>();
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            FolderPath = folderBrowser.ShowDialog() == DialogResult.OK ? folderBrowser.SelectedPath : string.Empty;

            CreateReportBtnIsEnabled = FolderPath != string.Empty;
        }

        /// <summary>
        /// Get all files from selected folder
        /// </summary>
        /// <param name="path">Selected folder path</param>
        /// <returns></returns>
        public List<string> GetAllFiles(string path)
        {
            ProgressBarVisibility = Visibility.Visible;
            var files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(Directory.GetFiles(directory, "*"));
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.MessageBox.Show("Нет доступа к системным файлам");
                return null;
            }
            ProgressBarVisibility = Visibility.Hidden;
            return files;
        }

        private string BuildHtmlReportDocument(IEnumerable<string> data)
        {
            IBuildDocument htmlReport = new HtmlReport<IEnumerable<string>>(data);

               return htmlReport.BuildDocument();
        }

        private void SaveAndOpenFile() 
        {
            if(string.IsNullOrEmpty(reportText)||string.IsNullOrWhiteSpace(reportText))
            {
                System.Windows.MessageBox.Show("Что-то пошло не так...");
            }

            //допустим пока так
            documentSettings = GetDocumentSettings(FileType.Html);

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = documentSettings.DefaultFileName;
            saveFileDialog.DefaultExt = documentSettings.DefaultExtention;
            saveFileDialog.Filter = documentSettings.DefaultFilter;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter file = new StreamWriter(saveFileDialog.FileName,false, Encoding.UTF8))
                    {
                        file.WriteLine(reportText);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                }
                    Process.Start(saveFileDialog.FileName);

                return;
            }
        }
        private IDocumentSettings GetDocumentSettings(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Html:
                    return new HtmlDocumentService();
                case FileType.Json:
                    return null;
                default:
                    return null;
            }
        }
        private async void CreateReport()
        {
            await Task.Run(() =>
            {
                if (FolderPath != string.Empty)
                {
                    
                    FileManipulateButtonsVisibility = Visibility.Hidden;

                    var files = GetAllFiles(FolderPath);//ас
                    ProgressBarVisibility = Visibility.Visible;
                    if (files is null)
                    {
                        ProgressBarVisibility = Visibility.Hidden;
                        return;
                    }
                    reportText = BuildHtmlReportDocument(files);//ас
                    ProgressBarVisibility = Visibility.Hidden;
                    FileManipulateButtonsVisibility = Visibility.Visible;
                }
            });
        }

        #endregion

        #region Commands
        public ICommand SelectFolderCommand { get; set; }

        public ICommand SaveOpenFileCommand { get; set; }

        public ICommand CreateReportCommand { get; set; }

        public ICommand FolderFieldChangedCommand { get; set; }
        #endregion
    }
    public enum FileType
    {
        Html,
        Json
            //и.т.д
    }
}
