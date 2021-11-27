using BeltsPack.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;

namespace BeltsPack.ViewModels
{
    public class AttachedImage
    {
        public string DisplayedImage { get; }
        public string Fullpath { get; }
        public string FancyName { get; }

        private string _thumbnailFilename;
        public string ThumbnailFilename
        {
            get
            {
                if (this._thumbnailFilename == null)
                {
                    this._thumbnailFilename = ImageHelper.CreateThumbnailFilename(this.Fullpath, 2048, 2048);
                }
                return this._thumbnailFilename;
            }
        }

        public AttachedImage(string fullpath, bool fromCamera = false)
        {
            
            this.Fullpath = fullpath;
            if (fromCamera)
            {
                this.FancyName = "Scatto fotocamera";
            }
            else
            {
                // extract just the filename from the fullpath
                this.FancyName = Path.GetFileName(this.Fullpath);
            }
        }
    }

    public class ReportProduzioneModel : INotifyPropertyChanged
    {
        
        // used an ObservableCollection instead of a List because of MVVM data-binding
        public ObservableCollection<AttachedImage> AttachedImages { get; set; }

        public ReportProduzioneModel()
        {

            this.AttachedImages = new ObservableCollection<AttachedImage>();
            // when the collection changes notify the EventHandler
            this.AttachedImages.CollectionChanged += AttachedImages_CollectionChanged;
        }

        public void AddImage(string filepath, bool fromCamera = false)
        {
            this.AttachedImages.Add(new AttachedImage(filepath, fromCamera));
        }

        public void RemoveImage(AttachedImage image)
        {
            this.AttachedImages.Remove(image);
        }

        #region MVVM related code

        private void AttachedImages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChange("AttachedImages");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }


}
