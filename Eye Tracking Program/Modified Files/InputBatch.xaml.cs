using System.Windows;

namespace UserPresenceWpf
{
    /// <summary>
    /// Interaction logic for InputBatch.xaml. Gets the input folder name for the image batch.
    /// </summary>
    partial class InputBatch : Window
    {
        
        public InputBatch()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prompts the user to enter the folder name of the images
        /// </summary>

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        /// <summary>
        /// Accepts the input folder name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
