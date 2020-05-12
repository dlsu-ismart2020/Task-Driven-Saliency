//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.ComponentModel;
using System.IO;
using EyeXFramework.Wpf;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;

namespace UserPresenceWpf
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class GazeData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _gazeX;
        private int _gazeY;
        private int _time;
        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public int gazeX
        {
            get
            {
                return _gazeX;
            }
            set
            {
                _gazeX = value;
                OnPropertyChanged("gazeX");
            }
        }
        public int gazeY
        {
            get
            {
                return _gazeY;
            }
            set
            {
                _gazeY = value;
                OnPropertyChanged("gazeY");
            }
        }
        public int time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged("time");
            }
        }
    }

    /// <summary>
    /// The main window for the image presentation
    /// </summary>

    public partial class MainWindow : Window
    {
        private WpfEyeXHost _eyeXHost;
        public GazeData publicGazeData = new GazeData();
        public bool userPresent;
        public string initialTime;
        public string outputFileDirectory, outputFileName, batchName;
        private DispatcherTimer timerImageChange;
        private Image[] ImageControls;
        private List<ImageSource> Images = new List<ImageSource>();
        private static string[] ValidImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private static string[] TransitionEffects = new[] { "Fade" };
        private string TransitionType, strImagePath = "";
        private int CurrentSourceIndex = -1, CurrentCtrlIndex, EffectIndex = 0, IntervalTimer = 1, IntervalRest = 1;
        private int count = 0;

        /// <summary>
        /// Initializes the EyeXHost, creates an Output directory, and loads the images based on the input folder name. 
        /// </summary>

        public MainWindow()
        {
            _eyeXHost = new WpfEyeXHost();
            _eyeXHost.Start();
            InitializeComponent();

            gazeDataTextX.DataContext = publicGazeData;
            gazeDataTextY.DataContext = publicGazeData;
            gazeDataTextTime.DataContext = publicGazeData;

            var stream = _eyeXHost.CreateGazePointDataStream(Tobii.EyeX.Framework.GazePointDataMode.LightlyFiltered);

            string exeRuntimeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            outputFileDirectory =
            Path.Combine(exeRuntimeDirectory, "Output");
                    if (!System.IO.Directory.Exists(outputFileDirectory))
                    {
                        // Output directory does not exist, so create it.
                        System.IO.Directory.CreateDirectory(outputFileDirectory);
                    }
            Console.WriteLine(exeRuntimeDirectory);
            Console.WriteLine(outputFileDirectory);
            var CSVDialog = new SaveCSVFile();
            if (CSVDialog.ShowDialog() == true)
            {
                outputFileName = CSVDialog.ResponseText;
            }
            var batchDialog = new InputBatch();
            if (batchDialog.ShowDialog() == true)
            {
                batchName = batchDialog.ResponseText;
            }
            File.WriteAllText(outputFileDirectory + @"\" + outputFileName + ".csv", "X Gaze Data, Y Gaze Data, Time \r\n");
            stream.Next += (s, e) => updateGazeData((int)e.X, (int)e.Y, (int)e.Timestamp);
            //Initialize Image control, Image directory path and Image timer.
            IntervalTimer = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"]);
            strImagePath = "Images/" + batchName;
            ImageControls = new[] { myImage, myImage2 };

            LoadImageFolder(strImagePath);

            timerImageChange = new DispatcherTimer();
            timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Updates the gaze data
        /// </summary>
        /// <param name="x">x coordinate of the  data</param>
        /// <param name="y">y coordinate of the data</param>
        /// <param name="time">time of the data</param>

        private void updateGazeData(int x, int y, int time)
        {
            publicGazeData.gazeX = x;
            publicGazeData.gazeY = y;
            publicGazeData.time = time;

            string csvFormattedGazeData = x.ToString() + "," + y.ToString() + "," + time.ToString();
            writeDataToFile(csvFormattedGazeData);
        }

        /// <summary>
        /// Writes the data to the output csv file
        /// </summary>
        /// <param name="text">contains the x coordinate, y coordinate, and time of the recorded gaze data</param>

        private void writeDataToFile(string text)
        {
            using (StreamWriter sw = File.AppendText(outputFileDirectory + @"\" + outputFileName + ".csv"))
            {
                sw.WriteLine(text);
            }
        }

        /// <summary>
        /// Loads the slideshow and enables the timer for each image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlaySlideShow();
            timerImageChange.IsEnabled = true;
        }

        /// <summary>
        /// Loads the folder based on the input folder name
        /// </summary>
        /// <param name="folder">the input folder name</param>

        private void LoadImageFolder(string folder)
        {
            ErrorText.Visibility = Visibility.Collapsed;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (!System.IO.Path.IsPathRooted(folder))
                folder = System.IO.Path.Combine(Environment.CurrentDirectory, folder);
            if (!System.IO.Directory.Exists(folder))
            {
                ErrorText.Text = "The specified folder does not exist: " + Environment.NewLine + folder;
                ErrorText.Visibility = Visibility.Visible;
                return;
            }
            var sources = from file in new System.IO.DirectoryInfo(folder).GetFiles().AsParallel()
                          where ValidImageExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase)
                          orderby file.FullName ascending
                          select CreateImageSource(file.FullName);
            Images.Clear();
            Images.AddRange(sources);
            sw.Stop();
        }

        /// <summary>
        /// Gets the next image to be shown
        /// </summary>
        /// <param name="file"></param>
        /// <returns>the next image to be shown</returns>

        private ImageSource CreateImageSource(string file)
        {
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(file, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            src.Freeze();
            return src;
        }

        /// <summary>
        /// Initializes the timer of the image based on its type (if wait time image or stimuli)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void timerImageChange_Tick(object sender, EventArgs e)
        {
            if (count % 2 == 1)
            {
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalRest);
                count++;
            }
            else
            {
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
                count++;
            }
            PlaySlideShow();
        }

        /// <summary>
        /// Shows the images on the screen
        /// </summary>

        private void PlaySlideShow()
        {
            try
            {
                if (Images.Count == 0)
                    return;
                if (count == 101)
                    System.Windows.Application.Current.Shutdown();
                var oldCtrlIndex = CurrentCtrlIndex;
                CurrentCtrlIndex = (CurrentCtrlIndex + 1) % 2;
                CurrentSourceIndex = (CurrentSourceIndex + 1) % Images.Count;

                Image imgFadeOut = ImageControls[oldCtrlIndex];
                Image imgFadeIn = ImageControls[CurrentCtrlIndex];
                ImageSource newSource = Images[CurrentSourceIndex];
                imgFadeIn.Source = newSource;

                TransitionType = TransitionEffects[EffectIndex].ToString();

                Storyboard StboardFadeOut = (Resources[string.Format("{0}Out", TransitionType.ToString())] as Storyboard).Clone();
                StboardFadeOut.Begin(imgFadeOut);
                Storyboard StboardFadeIn = Resources[string.Format("{0}In", TransitionType.ToString())] as Storyboard;
                StboardFadeIn.Begin(imgFadeIn);
                writeDataToFile(newSource.ToString());
            }
            catch (Exception ex) { }
        }
    }
}
