using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SoundPlay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ConfigObj Config { get; private set; }
        public static string RunLocal { get; private set; }
        public static string SoundLocal { get; private set; }
        public static MainWindow MainWindow_ { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RunLocal = AppContext.BaseDirectory;
            Config = ConfigUtils.Config(new ConfigObj
            {
                Sounds = new List<SoundItem>()
            }, RunLocal + "MainConfig.json");
            ReadList();
        }

        public static void ReadList()
        {
            SoundLocal = RunLocal + "Sounds\\";
            if (!Directory.Exists(SoundLocal))
            {
                Directory.CreateDirectory(SoundLocal);
            }
            var list = Directory.GetFiles(SoundLocal);
            var sounds = (from item in list select item.Replace(SoundLocal, "")).ToList();
            int MaxIndex = 0;
            foreach (var item in Config.Sounds)
            {
                if (MaxIndex < item.Index)
                {
                    MaxIndex = item.Index;
                }
                if (!sounds.Contains(item.Local))
                {
                    item.Have = false;
                }
                else
                {
                    sounds.Remove(item.Local);
                    item.Have = true;
                }
            }
            foreach (var item in sounds)
            {
                Config.Sounds.Add(new SoundItem
                {
                    Index = MaxIndex++,
                    AutoNext = false,
                    Have = true,
                    Local = item
                });
            }
            Update();
            Save();
        }
        public static void Update()
        {
            var query = from items in Config.Sounds orderby items.Index select items;
            var list = new List<SoundItem>(query);
            Config.Sounds.Clear();
            for (int add = 0; add < list.Count; add++)
            {
                var item = list[add];
                item.Index = add + 1;
                Config.Sounds.Add(item);
            }
        }
        public static void Save()
        {
            ConfigUtils.Save(Config, RunLocal + "MainConfig.json");
        }
    }
}
