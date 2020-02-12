using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Models
{
    public class BotUser:User
    {
        public string Password { get; set; }
        public int TimeElapsed { get; set; } = 1;

        public DateTime LastTimeOfLogin { get; set; }

        public bool IsEnabled { get; set; } = true;

        public string PhoneNumber { get; set; }
    }
}
