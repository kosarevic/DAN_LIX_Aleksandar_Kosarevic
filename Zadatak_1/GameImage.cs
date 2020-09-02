using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Zadatak_1
{
    //Fixed logical states for all images included in the game.
    enum GameState { NoneShowing, OneShowing, TwoShowing }
    enum ImageState { Hidden, Showing, Cleared }

    /// <summary>
    /// Class responsible for managing image objects behaviour while playing the game.
    /// </summary>
    class GameImage : Image
    {
        Uri hiddenUri;
        Uri clearedUri;
        BitmapImage hidden;
        BitmapImage cleared;
        BitmapImage showing;
        public readonly int ImageIndex;
        public ImageState State;

        /// <summary>
        /// Method responsible for different images states, during application life cycle.
        /// </summary>
        /// <param name="index"></param>
        public GameImage(int index)
        {
            ImageIndex = index;
            hiddenUri = new Uri("pack://application:,,,/images/hidden.png");
            clearedUri = new Uri("pack://application:,,,/images/cleared.png");
            Uri showingUri = new Uri("pack://application:,,,/images/car" + ImageIndex + ".png");
            hidden = new BitmapImage(hiddenUri);
            cleared = new BitmapImage(clearedUri);
            showing = new BitmapImage(showingUri);
            Hide();
        }
        //Method hides the real image with hidden.png.
        public void Hide()
        {
            State = ImageState.Hidden;
            this.Source = hidden;
        }
        //Method replaces found pair of real images with cleared.png.
        public void Clear()
        {
            State = ImageState.Cleared;
            this.Source = cleared;
        }
        //Method shows real image allocated to specific slot.
        public void Show()
        {
            State = ImageState.Showing;
            this.Source = showing;
        }
    }
}