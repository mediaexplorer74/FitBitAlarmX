﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataMonitorLib;
using System.IO;
using Utility;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Adaptive_Alarm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        int sleepTime;

        public List<string> deviceTypes = new List<string>(){ "None", "Fitbit" };
        public SettingsPage()
        {
            
            InitializeComponent();

            string saveFilename = Path.Combine(FileSystem.AppDataDirectory, "AppData.json");
            AppData data;
            string jsonstring;
            if (File.Exists(saveFilename))
            {
                jsonstring = File.ReadAllText(saveFilename);
                data = JsonConvert.DeserializeObject<AppData>(jsonstring);
            }
            else
            {
                data = new AppData();
            }
            SleepTimeNumber.Text = data.AwakeTime.ToString();

            typePicker.SelectedItem = Application.Current.Properties["CurrentDeviceType"];
            datasetStartLabel.Text = $"Current working dataset began: {((DateTime)Application.Current.Properties["CurrentDataSetStartDate"]).ToString("d")}";
        }

        private void ChangeDeviceButtonClicked(object sender, EventArgs e)
        {
            DataMonitor dataMonitor = null;

            if ((string)typePicker.SelectedItem == "Fitbit")
            {
                DataMonitor tentativeDM = new FitbitDataMonitor();
                Application.Current.Properties["tentativeDM"] = tentativeDM;
                ((FitbitDataMonitor)tentativeDM).Authenticate();
            }
            else
            {
                dataMonitor = new GaCDataMonitor();
            }
            Application.Current.Properties["CurrentDeviceType"] = (string)typePicker.SelectedItem;
            Application.Current.Properties["dataMonitor"] = dataMonitor;
        }

        private void saveButtonClicked(object sender, EventArgs e)
        {
            sleepTime = Convert.ToInt32(SleepTimeNumber.Text);
            string saveFilename = Path.Combine(FileSystem.AppDataDirectory, "AppData.json");
            AppData data;
            string jsonstring;
            if (File.Exists(saveFilename)){
                jsonstring = File.ReadAllText(saveFilename);
                data = JsonConvert.DeserializeObject<AppData>(jsonstring); 
            }
            else
            {
                data = new AppData();
            }
            data.AwakeTime = sleepTime;
            jsonstring = JsonConvert.SerializeObject(data);
            File.WriteAllText(saveFilename, jsonstring);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            typePicker.SelectedItem = Application.Current.Properties["CurrentDeviceType"];
            datasetStartLabel.Text = $"Current working dataset began: {((DateTime)Application.Current.Properties["CurrentDataSetStartDate"]).ToString("d")}";
        }
    }
}