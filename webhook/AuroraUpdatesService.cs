using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using DAL;
using webhook.Services;

namespace webhook
{
    //This class works with data from https://services.swpc.noaa.gov/text/aurora-nowcast-map.txt
    public class AuroraUpdatesService : IAuroraUpdatesService
    {
        private BackgroundWorker worker;
        private System.Timers.Timer timer = new System.Timers.Timer(5 * 60 * 100);

        public AuroraUpdatesService(IRepository repo, IBotService botService)
        {
            _repo = repo;
            _botService = botService;
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            _botService.Client.SendTextMessageAsync(86429548, "Updater started");
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private static WebClient client = new WebClient();

        private string cache = "";
        private readonly IRepository _repo;
        private readonly IBotService _botService;

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var newCache = client.DownloadString(path);
            _botService.Client.SendTextMessageAsync(86429548, "Downloaded file");
            if (newCache != cache)
            {
                cache = newCache;
                
                _botService.Client.SendTextMessageAsync(86429548, "Updates received");

                SendUpdates();
            }
        }

        private void SendUpdates()
        {
            foreach (var user in _repo.GetAllUserRecords())
            {
                if (user.Latitude.HasValue && user.Longitude.HasValue && GetProbability(user.Latitude.Value, user.Longitude.Value) > 10)
                {
                    _botService.Client.SendTextMessageAsync(user.ChatId, $"Aurora is likely in your area with P = {GetProbability(user.Latitude.Value, user.Longitude.Value):0.##}%");
                } 
            }
        }

        private const string path = "https://services.swpc.noaa.gov/text/aurora-nowcast-map.txt";

        public int GetProbability(double latitude, double longitude)
        {
            if (!worker.IsBusy)
                worker.RunWorkerAsync();
            worker.DoWork += worker_DoWork;

            const double k = 0.3515625;
            int row = 18 + (int)(k * longitude); //There are 18 lines of header strings in the files
            int col = (int)(k * (latitude + 90)); //Latitude varies from -90 to 90

            int probability = Convert.ToInt32(ReadAtPos(cache, row, col));

            return probability;
        }

        private static int ReadAtPos(string cache, int row, int col)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(cache);
            MemoryStream stream = new MemoryStream(byteArray);
            StreamReader file = new StreamReader(stream);

            // Read the file line by line
            string line;
            for (int counter = 0; (line = file.ReadLine()) != null; counter++)
            {
                if (counter == row)
                {
                    string[] split = line.Replace("   ", " ").Replace("  ", " ").Split(' ');

                    var foos = new List<string>(split);
                    foos.RemoveAt(0);
                    split = foos.ToArray();

                    return Convert.ToInt32(split[col]);
                }
            }

            return -1;
        }
    }
}