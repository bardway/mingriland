// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using TLAuto.Music.Controls;
#endregion

namespace TLAuto.Music.ServerHost.Models
{
    public class MusicInfo : ObservableObject
    {
        private string _filePath;

        private bool _isRepeat;

        private MusicStatusType _musicStatusType = MusicStatusType.Stop;

        private double _volume = 0.5;

        public MusicInfo(string mark)
        {
            Mark = mark;
            InitRemoveCommand();
        }

        public string Mark { get; }

        public string FilePath
        {
            set
            {
                _filePath = value;
                RaisePropertyChanged();
            }
            get => _filePath;
        }

        public double Volume
        {
            set
            {
                _volume = value;
                RaisePropertyChanged();
            }
            get => _volume;
        }

        public bool IsRepeat
        {
            set
            {
                _isRepeat = value;
                RaisePropertyChanged();
            }
            get => _isRepeat;
        }

        public MusicStatusType MusicStatusType
        {
            set
            {
                _musicStatusType = value;
                RaisePropertyChanged();
            }
            get => _musicStatusType;
        }

        public RelayCommand RemoveCommand { private set; get; }

        private void InitRemoveCommand()
        {
            RemoveCommand = new RelayCommand(OnRemoved);
        }

        public event EventHandler Removed;

        protected virtual void OnRemoved()
        {
            var handler = Removed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}