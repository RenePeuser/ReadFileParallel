using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ReadFileParallel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [ThreadStatic]
        private static FileInfo[] _files;

        private ConcurrentDictionary<string, string> _items;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var directoryInfo = new DirectoryInfo(@"d:\test");
            _items = new ConcurrentDictionary<string, string>();

            _files = directoryInfo.GetFiles(@"*.txt");

            foreach (var file in _files)
            {
                Check(file);
            }
            

            var stringList = new List<string>();
            foreach (var item in _items)
            {
                int testOut = 0;
                var result = int.TryParse(item.Key, out testOut);
                var outValue = "";
                var restest = _items.TryGetValue(item.Key, out outValue);
                stringList.Add(testOut + " - " + outValue);
            }

            stringList.Sort();
            listView.ItemsSource = stringList;

            MessageBox.Show("Finish");
        }

        private static object lockObject = new object();

        void Check(FileInfo file)
        {
            using (var streamReader = new StreamReader(file.FullName))
            {
                do
                {
                    var lineResult = streamReader.ReadLine();

                    if (!_items.ContainsKey(lineResult))
                    {
                        _items.AddOrUpdate(lineResult, file.Name, (key, value) => file.Name);                            
                    }

                } while (!streamReader.EndOfStream);
            }            
        }
    }
}
