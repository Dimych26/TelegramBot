using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramBot.BotContext;

namespace TelegramBot.Models.Registration
{
    public class RegistrationInBot
    {
        public static bool IsRegistrationCommand { get; set; } = false;

        public async static void Register(object sender, MessageEventArgs e)
        {
            
            var client = (TelegramBotClient)sender; 
            var chatId = e.Message.Chat.Id;
            if (Repository.GetUser(u => u.Id == chatId) != null)
            {
                await client.SendTextMessageAsync(chatId, "You are already registered");
                return;
            }


            await client.SendTextMessageAsync(chatId, "Create your password");
            IsRegistrationCommand = true;
        }

        public async static Task RegistrationConfirmed(object sender, MessageEventArgs e)
        {
            var client = (TelegramBotClient)sender;
            var user = e.Message.From;

            BotUser newUser = new BotUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                IsBot = user.IsBot,
                LanguageCode = user.LanguageCode,
                LastName = user.LastName,
                Password = Hash(e.Message.Text),
                Username = user.Username
            };

            Repository.GetContext().Add(newUser);
            await Repository.GetContext().SaveChangesAsync();

            await client.SendTextMessageAsync(newUser.Id, "You successfully registered");

            IsRegistrationCommand = false;
            return;
        }

        private static string Hash(string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(data);
            password = Convert.ToBase64String(result);
            return password;
        }
    }
}
