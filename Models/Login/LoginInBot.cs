using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BotContext;

namespace TelegramBot.Models.LoginUser
{
    public class LoginInBot
    {
        public static bool IsLoginCommand { get; set; } = false;

        public async static Task LoginConfirmed(object sender, MessageEventArgs e)
        {
            var client = (TelegramBotClient)sender;
            var chatId = e.Message.From.Id;
            BotUser currentUser = Repository.GetUser(u => u.Id == chatId);

            if (currentUser.Password == Hash(e.Message.Text))
            {
                IsLoginCommand = false;
                await client.SendTextMessageAsync(chatId, $"Hello {currentUser.FirstName}", replyMarkup:GetKeyboard());
                currentUser.LastTimeOfLogin = DateTime.Now;
                Repository.GetContext().Update(currentUser);
                await Repository.GetContext().SaveChangesAsync();
                return;
            }

                await client.SendTextMessageAsync(chatId, "Incorrect password!");
        }

        public async static void LoginUser(object sender, MessageEventArgs e)
        {
            var client = (TelegramBotClient)sender;
            var chatId = e.Message.Chat.Id;
            var user = Repository.GetUser(u => u.Id == chatId);
            

            if (user != null)
            {
                IsLoginCommand = true;
                await client.SendTextMessageAsync(chatId, "Enter your password");
                return;
            }

            await client.SendTextMessageAsync(chatId, "You are not exists in database!");
        }

        public static bool CheckSession(long chatId)
        {
            var user = Repository.GetUser(u => u.Id == chatId);
            if (user != null)
            {
                var date = DateTime.Now - user.LastTimeOfLogin;
                if (date.Minutes < user.TimeElapsed)
                {
                    return true;
                }
            }

            return false;
        }

        private static ReplyKeyboardMarkup GetKeyboard()
        {

            var rkm = new ReplyKeyboardMarkup();

            rkm.Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Send your contact") { RequestContact = true }
                }
            };

            return rkm;
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
