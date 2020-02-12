using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;

namespace TelegramBot
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            Bot.Start();
            CreateHostBuilder(args).Build().Run();
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });



        //private async static void BotOnMessageReceived(object sender, MessageEventArgs e)
        //{

        //    var message = e.Message;

        //    var client = Bot.GetTelegramClient();

        //    try
        //    {
        //        var command = Bot.Commands.Find(match => match.Equals(message));

        //        await command.Execute(message, client);
        //    }
        //    catch(NullReferenceException)
        //    {
        //        await client.SendTextMessageAsync(message.Chat.Id, "Такой команды нет, повторите ввод");
        //    }
        //    catch(ArgumentNullException)
        //    {
        //        await client.SendTextMessageAsync(message.Chat.Id, "Команд пока нет, следите за обновлениями");
        //    }



        //}
    }
}
