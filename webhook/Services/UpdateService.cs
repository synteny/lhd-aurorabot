﻿using System;
using System.Threading.Tasks;
using DAL;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;

namespace webhook.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;
        private readonly IRepository _repository;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IRepository repo)
        {
            _botService = botService;
            _logger = logger;
            _repository = repo;
        }

        public Task Respond(Update update)
        {
            return new Task(
               async () =>
            {
                int user_id = update.Message.From.Id;
                UserRecord userRecord = _repository.GetUserRecord(user_id);
                if (userRecord == null) // && update.Message.Text == "/start")
                {
                    // 0 stage
                    _repository.AddUserRecord(update.Message.From.Id);
                    await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Hello!\nEnter your location.");
                }
                else if (userRecord != null) // && userRecord.DialogState == 1)
                {
                    // 1 stage
                    string app_id = "Xnse0BiuhZ1dXBiF3bPB";
                    string app_code = "BKh3_vJ5dwUucVnUsfGTTA";
                    try {
                        string json = GET("https://geocoder.api.here.com/6.2/geocode.json", $"app_id={app_id}&app_code={app_code}&searchtext={update.Message.Text}");

                        JObject obj = JObject.Parse(json);

                        var position = obj["Response"]["View"][0]["Result"][0]["Location"]["NavigationPosition"][0];

                        double latitude, longitude;
                        if (double.TryParse(position["Latitude"].ToString(), out latitude)
                            && double.TryParse(position["Longitude"].ToString(), out longitude))
                        {
                            userRecord.latitude = latitude;
                            userRecord.longitude = longitude;
                            userRecord.dialog_state += 1;
                            _repository.UpdateUserRecord(userRecord);

                            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                "Your location is set to: " + latitude + "; " + longitude);
                        }
                        else
                        {
                            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Can't find your position");
                        }
                    }
                    catch (Exception)
                    {
                        await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Can't find your position");
                    }
                }

            });
        }

        private static string GET(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
    }
}
