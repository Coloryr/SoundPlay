using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SoundPlay
{
    class ViewModelCommand : ICommand
    {
        public Action<object> DoExcute;
        public Func<object, bool> CanExcute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExcute?.Invoke(parameter) ?? false;
        }

        public void Execute(object parameter)
        {
            DoExcute?.Invoke(parameter);
        }
    }
    class ViewModel
    {
        private string SoundName_;
        private int Index_;
        private string TimeAll_;
        private string TimeNow_;
        private string NowUrl_;
        private ObservableCollection<SoundItem> Sounds_;
        public string SoundName
        {
            get => SoundName_;
            set => SetProperty(ref SoundName_, value);
        }
        public int Index
        {
            get => Index_;
            set => SetProperty(ref Index_, value);
        }
        public string TimeAll
        {
            get => TimeAll_;
            set => SetProperty(ref TimeAll_ ,value);
        }
        public string TimeNow
        {
            get => TimeNow_;
            set => SetProperty(ref TimeNow_ ,value);
        }
        public string NowUrl
        {
            get => NowUrl_;
            set => SetProperty(ref NowUrl_, value);
        }

        public ObservableCollection<SoundItem> Sounds
        {
            get => Sounds_;
            set => SetProperty(ref Sounds_, value);
        }
        private MediaElement MediaElementObject_;
        public MediaElement MediaElementObject
        {
            get => MediaElementObject_;
            set => SetProperty(ref MediaElementObject_, value);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand ButtonClick
        {
            get
            {
                return new ViewModelCommand()
                {
                    DoExcute = new Action<object>(ButtonAction),
                    CanExcute = new Func<object, bool>(CanAction)
                };
            }
        }

        private bool CanAction(object arg)
        {
            return !(arg is null);
        }

        private void ButtonAction(object obj)
        {
            switch (obj as string)
            {
                case "Start":
                    if (IsPlay)
                    {
                        return;
                    }
                    var list = from item in Sounds_ where item.Index == Index select item;
                    if (!list.Any())
                    {
                        MessageBox.Show("播放错误");
                        return;
                    }
                    PlayItem = list.First();
                    NowUrl_ = PlayItem.Local;

                    break;
            }
        }

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private bool IsPlay;
        private SoundItem PlayItem;

        public ViewModel()
        {
            SoundName_ = "无";
            Index_ = 1;
            TimeAll_ = "00:00";
            TimeNow_ = "00:00";
            Sounds_ = new ObservableCollection<SoundItem>(App.Config.Sounds);
            NowUrl_ = "";
        }
    }
}
