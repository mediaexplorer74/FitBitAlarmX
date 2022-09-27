﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using GuessCheck;

namespace Adaptive_Alarm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        AppData appData;
        string saveFilename;
        INotificationManager notificationManager;
        int notificationNumber = 0;

        //public string wakeUpTime { get; } = "Waking you up at";

        public  MainPage()
        {
            InitializeComponent();
            saveFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AppData.json");
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

            if (File.Exists(saveFilename))
            {
                string jsonstring = File.ReadAllText(saveFilename);
                appData = JsonConvert.DeserializeObject<AppData>(jsonstring);
            }
            else
            {
                appData = new AppData();
            }
            TPMonday.Time = appData.monday;
            TPTuesday.Time = appData.tuesday;
            TPWednesday.Time = appData.wednesday;
            TPThursday.Time = appData.thursday;
            TPFriday.Time = appData.friday;
            TPSaturday.Time = appData.saturday;
            TPSunday.Time = appData.sunday;

            
            

            if((DateTime.Now - appData.nextChanged).TotalHours > 16){

                appData.next = appData.currTimeSpan();

            }

            TPNext.Time = appData.next;

            if((DateTime.Now - appData.scoreAdded).TotalHours > 16)
            {
                ScorePrompt();
            }
             
            /*
            TPMonday.PropertyChanged += "OnTimePickerPropertyChanged";
            TPTuesday.Time = appData.tuesday;
            TPWednesday.Time = appData.wednesday;
            TPThursday.Time = appData.thursday;
            TPFriday.Time = appData.friday;
            TPSaturday.Time = appData.saturday;
            TPSunday.Time = appData.sunday;*/

        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                stackLayout.Children.Add(msg);
            });
        }

        private async void ScorePrompt()
        {
            HashSet<string> acceptableScores = new HashSet<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
            string result = await DisplayPromptAsync("Wakefulness", "How rested did you feel waking up this morning?", placeholder:"Scale 1-10 where 10 is best", maxLength:2, keyboard:Keyboard.Numeric);
          
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = result.Trim();
                while (!acceptableScores.Contains(result))
                {
                    result = await DisplayPromptAsync("Wakefulness", "Please input a number 1-10", placeholder: "Scale 1-10 where 10 is best", maxLength: 2, keyboard: Keyboard.Numeric);
                    if (!string.IsNullOrWhiteSpace(result)){
                        result = result.Trim(); 
                    }
                }
                int score = Convert.ToInt32(result);
                GaC.addScore(score);
                appData.scoreAdded = DateTime.Now;
            }
        }
        async void OnSleepPressed(object sender, EventArgs e)
        {
            if (File.Exists(saveFilename))
            {
                string jsonstring = File.ReadAllText(saveFilename);
                appData = JsonConvert.DeserializeObject<AppData>(jsonstring);
            }
            else
            {
                appData = new AppData();
            }
            int totalMin = GaC.findAlarmTime(appData.currDateTime(), appData.AwakeTime);
            DateTime nTime = DateTime.Now;
            TimeSpan time = TimeSpan.FromMinutes(totalMin);
            DateTime wakeTime = nTime + time;
            //notificationManager.SendNotification("succes?", string.Format("{0:hh:mm tt}", wakeTime));
            //notificationManager.SendNotification("test", "1 min later", DateTime.Now.AddMinutes(1));
            notificationManager.SendNotification("WAKE UP", "IT IS TIME TO WAKE UP", wakeTime.AddMinutes(-1));
            string message = "Initial Alarm set for " + string.Format("{0:hh:mm tt}", wakeTime) 
                + " To wake up before " + appData.currDateTime().ToString();
            //TimeMessage.Text = message;
            await DisplayAlert("Reminder", message, "OK");
        }

        async void OnScorePressed(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScorePage());
        }

        void OnTimePickerPropertyChangedM(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.monday = TPMonday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedTu(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.tuesday = TPTuesday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedW(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.wednesday = TPWednesday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedTh(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.thursday = TPThursday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedF(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.friday = TPFriday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedSa(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.saturday = TPSaturday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedSu(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.sunday = TPSunday.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
            }
        }

        void OnTimePickerPropertyChangedNe(object sender, PropertyChangedEventArgs args)
        {
            // Saves all the times to files
            if (args.PropertyName == "Time")
            {
                appData.next = TPNext.Time;
                string jsonstring = JsonConvert.SerializeObject(appData);
                File.WriteAllText(saveFilename, jsonstring);
                appData.nextChanged = DateTime.Now;
            }
        }

    }
}                       