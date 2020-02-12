using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using TelegramBot.BotContext;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {

        static  BotController()
        {
            Bot.AlarmEvent += SendMessageAll;
            Bot.CommandEvents["/send"] += SendMessageCommand;
            
        }

        private static async void SendMessageCommand(object sender, MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;

            await Bot.SendTextMessage(chatId, "Send Message");
        }

        private static void SendMessageAll(string message)
        {
            var listOfUsers = Repository.GetContext().Users.ToList();
            listOfUsers.ForEach(async (s) => await Bot.SendTextMessage(s.Id, "ALARM"));
        }

        [HttpGet]
        [Route("Get")]
        public void DoAlarm()
        {          
            //Bot.SendAlarm();
        }
        
        [HttpPost]
        [Route("Send")]
        public async Task<IActionResult> SendMessage()
        {
            var bot = Bot.GetTelegramClient();

            var s = await bot.GetUpdatesAsync();

            foreach(var update in s)
            {
                await Bot.SendTextMessage(update.Message.Chat.Id, update.Message.Text);
            }

            return Ok();
        }

        [HttpPost]
        [Route("SendPhoto")]
        public async Task<IActionResult> SendPhoto([FromBody]byte[] encodePhoto)
        {
            if (encodePhoto != null)
            {
                using (Stream stream = new MemoryStream(encodePhoto))
                {
                    InputOnlineFile photo = new InputOnlineFile(stream);
                    var Id = Bot.ListOfChatId.FirstOrDefault();

                    await Bot.SendPhoto(Id, photo);
                }
                return Ok();
            }
            return BadRequest("The photo is not valid");
        }

        [HttpPost]
        [Route("BlockUser")]
        public IActionResult BlockUser(string phoneNumber)
        {
            if (phoneNumber == null)
            {
                return BadRequest("PhoneNumber is null");

            }
            phoneNumber = phoneNumber.Replace("+", "");
            phoneNumber = phoneNumber.Replace(" ", "");
            var result = Bot.BlockUser(phoneNumber);

            if (result)
                return Ok();
            return BadRequest("Phone number is not exist in database!");
        }

        [HttpPost]
        [Route("UnBlockUser")]
        public IActionResult UnBlockUser(string phoneNumber)
        {
            if (phoneNumber == null)
            {
                return BadRequest("PhoneNumber is null");

            }
            phoneNumber = phoneNumber.Replace("+", "");
            phoneNumber = phoneNumber.Replace(" ", "");
            var result = Bot.UnBlockUser(phoneNumber);

            if (result)
                return Ok();
            return BadRequest("Phone number is not exist in database!");
        }

        [HttpPost]
        [Route("SendInfo")]
        public async Task<IActionResult> SendInfo(string p, string c)
        {
            try
            {
                if (p == null)
                    return BadRequest("PhoneNumber is null");

                if (c == null)
                    return BadRequest("child is null");

                var chatId = Bot.GetUser(u => u.PhoneNumber == p).Id;
                await Bot.SendTextMessage(chatId, c);


                return Ok();
            }

            catch(KeyNotFoundException)
            {
                return BadRequest("Phone number is not exist in database!");
            }
        }
    }
}