using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameManager
{
    class ImageWrapper : System.Windows.Controls.ContentControl
    {

        public ImageWrapper()
        {

            this.Unloaded += ImageWrapper_Unloaded;
        }

        private void ImageWrapper_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {

            var image = this.Content as System.Windows.Controls.Image;

            if (image != null)
            {

                image.Source = null;
            }
        }
    }
}
