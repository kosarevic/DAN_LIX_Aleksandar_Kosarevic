using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Zadatak_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Array is created to be populated with all images withing the game.
        GameImage[] gameImages = new GameImage[16];
        //GameState set to no images showing, when the application starts.
        GameState state = GameState.NoneShowing;
        //Number represents all cleared images, with maximum value of 8 (representing discovered pairs of 16 images).
        int numCleared = 0;
        //Static timer variable, representing avalaible time to the player to complete the puzzle.
        public static int timer = 61;
        //Array stores the clicked images objects, for latter comparison.
        int[] imgsClicked = new int[2];
        //Timer made to calculate elapsed time, while using the application.
        DispatcherTimer clockTimer = new DispatcherTimer();
        //Timer made to create delay when images are interacted with.
        DispatcherTimer waitTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            CreateBorders();
            Reset();
            clockTimer.Tick += Timer_Tick;
            clockTimer.Interval = new TimeSpan(0, 0, 1);
            waitTimer.Tick += Timer_Wait;
            waitTimer.Interval = new TimeSpan(0, 0, 0, 0, 700);
        }

        /// <summary>
        /// Method responsible for disabling user interaction when two images are selected for comparison.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Wait(object sender, EventArgs e)
        {
            waitTimer.Stop();
            //Images that are clicked by the user are stored in following variables.
            var bd1 = ImageGrid.Children[imgsClicked[0]] as Border;
            var img1 = bd1.Child as GameImage;
            var bd2 = ImageGrid.Children[imgsClicked[1]] as Border;
            var img2 = bd2.Child as GameImage;
            //If images match, they are cleared and replaced with cleared.png image, else they remain hidden with hidden.png image.
            if (img1.ImageIndex == img2.ImageIndex)
            {
                img1.Clear();
                img2.Clear();
                numCleared++;
            }
            else
            {
                img1.Hide();
                img2.Hide();
            }
            //State of the game is set to "NonShowing" when above comparison is made, so that life cycle can continue.
            state = GameState.NoneShowing;
            //If all 8 pairs of the matching images are discovered, player gets a wictory message which is logged into a file.
            if (numCleared == 8)
            {
                clockTimer.Stop();
                int TotalTime = 61 - (int)ElapsedTime.Content;
                MessageBox.Show(String.Format("Congratulations! You Have won the game!\n\nElapsed Time: {0} seconds", TotalTime));
                timer = 61;
                clockTimer.Stop();
                //File logging is realised below.
                string createText = DateTime.Now + ", Total time: " + TotalTime.ToString() + " seconds" + Environment.NewLine;
                File.AppendAllText(@"..\\..\Files\IgraPamcenja.txt", createText);
                //Player is asked if he/she wishes to play again.
                if (MessageBox.Show("Do you want to play again?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MainWindow window = new MainWindow();
                    window.Show();
                    Close();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }
        /// <summary>
        /// Method responsible for reducing avalaible time to the user after each second passes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Window timer display is represented with this variable.
            ElapsedTime.Content = --timer;
            //If "timer" variable reaches 0, game informs the user that time has run out and restarts again.
            if (timer == 0)
            {
                MessageBox.Show(String.Format("Time has run out, try again."));
                timer = 61;
                clockTimer.Stop();
                MainWindow window = new MainWindow();
                window.Show();
                Close();
            }
        }
        /// <summary>
        /// Method resposible for styling the user interface.
        /// </summary>
        private void CreateBorders()
        {
            for (int i = 0; i < gameImages.Length; i++)
            {
                var bd = new Border();
                bd.BorderThickness = new Thickness(1);
                bd.BorderBrush = Brushes.Black;
                bd.Padding = new Thickness(5);
                bd.Margin = new Thickness(5);
                bd.Tag = i;
                ImageGrid.Children.Add(bd);
            }
        }

        /// <summary>
        /// Method resposible for randomasing images location on user interface.
        /// </summary>
        private void SetImages()
        {
            var rand = new Random();
            var stack = new Stack<int>();
            while (stack.Count < 8)
            {
                var r = rand.Next(8);
                if (!stack.Contains(r)) stack.Push(r);
            }
            //Images are alocated from the random Stack collection.
            foreach (int idx in stack)
            {
                var r = rand.Next(16);
                while (gameImages[r] != null) r = rand.Next(16);
                gameImages[r] = new GameImage(idx);
                while (gameImages[r] != null) r = rand.Next(16);
                gameImages[r] = new GameImage(idx);
            }
        }
        /// <summary>
        /// Method resets the values to default when game initiates again.
        /// </summary>
        private void Reset()
        {
            timer = 61;
            numCleared = 0;
            clockTimer.Start();
            SetImages();
            for (int i = 0; i < gameImages.Length; i++)
            {
                var bd = ImageGrid.Children[i] as Border;
                bd.Child = gameImages[i];
            }
        }
        /// <summary>
        /// Method called when user clicks on any avalaible field of images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Clicked(object sender, InputEventArgs e)
        {
            var bd = sender as Border;
            var img = bd.Child as GameImage;
            //Switch added to set the application behaviour when images are interacted with.
            switch (state)
            {
                case GameState.NoneShowing:
                    img.Show();
                    imgsClicked[0] = (int)bd.Tag;
                    state = GameState.OneShowing;
                    break;
                case GameState.OneShowing:
                    if (imgsClicked[0] == (int)bd.Tag) return;
                    imgsClicked[1] = (int)bd.Tag;
                    img.Show();
                    state = GameState.TwoShowing;
                    waitTimer.Start();
                    break;
            }
        }
    }
}
