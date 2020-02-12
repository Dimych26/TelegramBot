using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Models;

namespace TelegramBot.BotContext
{
    public class Repository
    {
        private static BotApplicationContext context;

        public static BotApplicationContext GetContext()
        {           
            if (context == null)
                context = new BotApplicationContext();

            return context;
        }

        public static BotUser GetUser(Expression<Func<BotUser, bool>> predicate)
        {
            var user = GetContext().Users.FirstOrDefault(predicate);
            return user;
        }
    }
}
