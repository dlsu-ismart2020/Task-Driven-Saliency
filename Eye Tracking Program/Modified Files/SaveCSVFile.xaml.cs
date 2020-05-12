using System.Windows;

namespace UserPresenceWpf
{
    /// <summary>
    /// Interaction logic for SaveCSVFile.xaml
    /// </summary>
    partial class SaveCSVFile : Window
    {

        public SaveCSVFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prompts the user to input the output csv filename
        /// </summary>

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        /// <summary>
        /// Accepts the input filename
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
