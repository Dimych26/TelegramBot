using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBot.Models.KeyBoard;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BotContext;
using TelegramBot.Events;
using TelegramBot.Models.Commands;
using TelegramBot.Models.LoginUser;
using TelegramBot.Models.Registration;
using TelegramBot.Models.BotUserService;
using System.Linq.Expressions;

namespace TelegramBot.Models
{
    public class Bot
    {
        private static TelegramBotClient client;

        public static List<long> ListOfChatId;

        public static Dictionary<string,long> ListOfContacts;

        public static event Action<string> AlarmEvent;

        public static Dictionary<string, Action<object, MessageEventArgs>> CommandEvents;

        public static void Start()
        {
            client = new TelegramBotClient(AppSettings.Token);
            ListOfChatId = new List<long>();
            ListOfContacts = new Dictionary<string, long>();
            CommandEvents = new Dictionary<string, Action<object, MessageEventArgs>>();
            AddCommandEvents();
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
        }

        private async static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                CommandEvents[e.Message.Text](sender, e);
            }
            catch (KeyNotFoundException)
            {
                if (RegistrationInBot.IsRegistrationCommand)
                {
                    await RegistrationInBot.RegistrationConfirmed(sender, e);
                    await LoginInBot.LoginConfirmed(sender, e);
                    return;
                }

                if (LoginInBot.IsLoginCommand)
                {
                    await LoginInBot.LoginConfirmed(sender, e);
                    return;
                }
                var chatId = e.Message.Chat.Id;
                var user = GetUser(u => u.Id == e.Message.From.Id);

              
                InformUserAboutBlock(chatId);

                bool IsSessionExists = CheckSession(chatId);
                if (IsSessionExists)
                    await client.SendTextMessageAsync(e.Message.Chat.Id, "Not found this command");
                
            }
            catch (ArgumentNullException) 
            {
                SendInfo(e.Message.Chat.Id, e);
            }
        }

        public static void InformUserAboutBlock(long chatId)
        {
            var isBloked = IsUserBlock(chatId);

            if (isBloked)
            {
                client.SendTextMessageAsync(chatId, "You are blocked").Wait();
                return;
            }
        }

        private static bool CheckSession(long chatId)
        {
            bool isExist = IsUserExists(chatId);
            if (isExist)
            {
                bool isBlock = IsUserBlock(chatId);
                if (!isBlock)
                {
                    bool IsSessionExists = LoginInBot.CheckSession(chatId);
                    if (!IsSessionExists)
                        client.SendTextMessageAsync(chatId, "Your session is over!", replyMarkup: new ReplyKeyboardRemove()).Wait();
                    return IsSessionExists;
                }
            }
            return false;
        }

        private static bool IsUserBlock(long Id)
        {
            return UserService.IsUserBlock(Id);
        }

        private static bool IsUserExists(long chatId)
        {
            var user = GetUser(u => u.Id == chatId);
            if (user == null)
            {
                client.SendTextMessageAsync(chatId, "This user not allowed, please contact with administrator").Wait();
                return false; 
            }
            return true;
        }

        public static bool BlockUser(string phoneNumber)
        {
            return UserService.BlockUser(phoneNumber);       
        }

        public static bool UnBlockUser(string phoneNumber)
        {
            return UserService.UnBlockUser(phoneNumber);
        }


        public static Task SendTextMessage(long chatId, string message)
        {
            return client.SendTextMessageAsync(chatId, message);
        }    

        public static void SendInfo(long chatId, MessageEventArgs e)
        {
            string phoneNumber = null; 
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Contact)
            {
                phoneNumber = e.Message.Contact.PhoneNumber.Replace("+","");
                var user = Repository.GetContext().Users.FirstOrDefault(u => u.Id == chatId);
                if (user.PhoneNumber == null)
                {
                    user.PhoneNumber = phoneNumber;
                    Repository.GetContext().Update(user);
                    Repository.GetContext().SaveChanges();
                }

            }
        }

        private static void AddCommandEvents()
        {
            CreateCommand();
            CommandEvents["/start"] += StartCommand;
            CommandEvents["/register"] += RegistrationInBot.Register;
            CommandEvents["/login"] += LoginInBot.LoginUser;
        }

        private static void CreateCommand()
        {
            CommandEvents.Add("/start", null);
            CommandEvents.Add("/send", null);
            CommandEvents.Add("/register", null);
            CommandEvents.Add("/login", null);
            CommandEvents.Add("/remove", null);
        }

        private static async void StartCommand(object sender, MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;

            //var user = Repository.GetUser(u => u.Id == chatId);

            //IsUserExists(chatId);

            InformUserAboutBlock(chatId);

            bool IsSessionExists = CheckSession(chatId);

            if(IsSessionExists)
            await client.SendTextMessageAsync(chatId, "Start command!");

        }

        

        public static void SendAlarm()
        {
            AlarmEvent?.Invoke("ALARM");
        }

        private static void SendMessageAll()
        {
            var listOfUsers = Repository.GetContext().Users.ToList();
            listOfUsers.ForEach(async (s) => await client.SendTextMessageAsync(s.Id, "ALARM"));
        }

        public static async Task SendPhoto(long Id, InputOnlineFile photo)
        {
            await client.SendPhotoAsync(Id, photo);
        }

        public static TelegramBotClient GetTelegramClient()
        {
            if (client != null)
                return client;

            client = new TelegramBotClient(AppSettings.Token);
           
            return client;
        }

        public static BotUser GetUser(Expression<Func<BotUser, bool>> predicate)
        {
            return Repository.GetUser(predicate);
        }

       

        //private static ReplyKeyboardMarkup GetKeyboard()
        //{
            
        //    var rkm = new ReplyKeyboardMarkup();

        //    rkm.Keyboard = new KeyboardButton[][]
        //    {
        //        new KeyboardButton[]
        //        {
        //            new KeyboardButton("Send your contact") { RequestContact = true }
        //        }
        //    };

        //    return rkm;
        //}
    }
}
