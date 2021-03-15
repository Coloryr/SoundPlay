using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
    class ViewModel : INotifyPropertyChanged
    {
        private string SoundName_;
        private int Index_;
        private string TimeAll_;
        private string TimeNow_;
        private ObservableCollection<SoundItem> Sounds_;
        private string State_;
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
            set => SetProperty(ref TimeAll_, value);
        }
        public string TimeNow
        {
            get => TimeNow_;
            set => SetProperty(ref TimeNow_, value);
        }

        public string State
        {
            get => State_;
            set => SetProperty(ref State_, value);
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
        public ICommand UpClick
        {
            get
            {
                return new ViewModelCommand()
                {
                    DoExcute = new Action<object>((obj) =>
                    {
                        int index = (int)obj;
                        if (index <= 0)
                        {
                            return;
                        }
                        SoundItem item = App.Config.Sounds[index];
                        SoundItem item1 = App.Config.Sounds[index - 1];
                        int temp = item.Index;
                        item.Index = item1.Index;
                        item1.Index = temp;
                        App.Update();
                        App.Save();
                        Sounds.Clear();
                        foreach (var item2 in App.Config.Sounds)
                        {
                            Sounds.Add(item2);
                        }
                    }),
                    CanExcute = new Func<object, bool>(CanAction)
                };
            }
        }
        public ICommand DownClick
        {
            get
            {
                return new ViewModelCommand()
                {
                    DoExcute = new Action<object>((obj) =>
                    {
                        int index = (int)obj;
                        if (index >= Sounds_.Count - 1)
                        {
                            return;
                        }
                        SoundItem item = App.Config.Sounds[index];
                        SoundItem item1 = App.Config.Sounds[index + 1];
                        int temp = item.Index;
                        item.Index = item1.Index;
                        item1.Index = temp;
                        App.Update();
                        App.Save();
                        Sounds.Clear();
                        foreach (var item2 in App.Config.Sounds)
                        {
                            Sounds.Add(item2);
                        }
                    }),
                    CanExcute = new Func<object, bool>(CanAction)
                };
            }
        }

        public ICommand AutoClick
        {
            get
            {
                return new ViewModelCommand()
                {
                    DoExcute = new Action<object>((obj) =>
                    {
                        int index = (int)obj;
                        if (index < 0)
                        {
                            return;
                        }
                        SoundItem item = App.Config.Sounds[index];
                        item.AutoNext = !item.AutoNext;
                        App.Save();
                        Sounds.Clear();
                        foreach (var item2 in App.Config.Sounds)
                        {
                            Sounds.Add(item2);
                        }
                    }),
                    CanExcute = new Func<object, bool>(CanAction)
                };
            }
        }

        public ICommand DeleteClick
        {
            get
            {
                return new ViewModelCommand()
                {
                    DoExcute = new Action<object>((obj) =>
                    {
                        int index = (int)obj;
                        if (index < 0)
                        {
                            return;
                        }
                        App.Config.Sounds.RemoveAt(index);
                        App.Update();
                        App.Save();
                        Sounds.Clear();
                        foreach (var item2 in App.Config.Sounds)
                        {
                            Sounds.Add(item2);
                        }
                    }),
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
                    if (!IsPlay)
                    {
                        var list = from item in Sounds_ where item.Index == Index select item;
                        if (!list.Any())
                        {
                            MessageBox.Show("播放错误");
                            return;
                        }
                        PlayItem = list.First();
                        MediaElementObject_.Source = new Uri(App.SoundLocal + PlayItem.Local);
                        MediaElementObject_.Play();
                        SoundName = PlayItem.Local;
                        IsPlay = true;
                        State = "播放";
                    }
                    else if (IsPause)
                    {
                        MediaElementObject_.Play();
                        IsPause = false;
                    }
                    break;
                case "Pause":
                    if (!IsPlay || IsPause)
                    {
                        return;
                    }
                    State = "暂停";
                    MediaElementObject_.Pause();
                    IsPause = true;
                    break;
                case "Reset":
                    if (!IsPlay)
                    {
                        return;
                    }
                    State = "停止";
                    MediaElementObject_.Stop();
                    IsPlay = false;
                    IsPause = false;
                    break;
                case "First":
                    MediaElementObject_.Stop();
                    Clear();
                    Index = 1;
                    break;
                case "Last":
                    MediaElementObject_.Stop();
                    Clear();
                    if (Index <= 1)
                    {
                        return;
                    }
                    Index--;
                    break;
                case "Next":
                    MediaElementObject_.Stop();
                    Clear();
                    if (Index >= Sounds.Count)
                    {
                        return;
                    }
                    Index++;
                    break;
                case "Refresh":
                    MediaElementObject_.Stop();
                    Clear();
                    Index = 1;
                    App.ReadList();
                    Sounds = new ObservableCollection<SoundItem>(App.Config.Sounds);
                    break;
            }
        }

        private void Clear()
        {
            TimeAll = "00:00";
            TimeNow = "00:00";
            State = "停止";
            SoundName = "无";
            IsPlay = false;
            IsPause = false;
        }

        private bool IsPlay;
        private bool IsPause;
        private SoundItem PlayItem;
        private Thread RunTask;
        private bool IsRun;

        public ViewModel()
        {
            SoundName_ = "无";
            Index_ = 1;
            TimeAll_ = "00:00";
            TimeNow_ = "00:00";
            State_ = "停止";
            Sounds_ = new ObservableCollection<SoundItem>(App.Config.Sounds);
            MediaElementObject_ = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            MediaElementObject_.MediaEnded += MediaElementObject_MediaEnded;
            App.MainWindow_.Closing += MainWindow_Closing;
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            RunTask = new Thread(() =>
            {
                while (IsRun)
                {
                    if (IsPlay)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            if (MediaElementObject.NaturalDuration.HasTimeSpan)
                            {
                                TimeSpan timeall = MediaElementObject.NaturalDuration.TimeSpan;
                                TimeAll = $"{timeall.Minutes:00}:{timeall.Seconds:00}";
                            }
                            TimeSpan time = MediaElementObject_.Position;
                            TimeNow = $"{time.Minutes:00}:{time.Seconds:00}";
                        }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
                    }
                    Thread.Sleep(200);
                }
            });
            IsRun = true;
            RunTask.Start();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            IsRun = false;
            if (IsPlay)
            {
                MediaElementObject_.Stop();
            }
        }

        private void MediaElementObject_MediaEnded(object sender, RoutedEventArgs e)
        {
            IsPlay = false;
            Index++;
            Clear();
            if (PlayItem.AutoNext)
            {
                ButtonAction("Start");
            }
        }
    }
}
