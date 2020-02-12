using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Models.KeyBoard
{
    public class BotKeyBoard
    {
        public static ReplyKeyboardMarkup GetKeyboard()
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
    }
}
