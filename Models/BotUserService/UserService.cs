using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.BotContext;

namespace TelegramBot.Models.BotUserService
{
    public class UserService
    {
        public static bool IsUserBlock(long Id)
        {
            var user = Repository.GetContext().Users.FirstOrDefault(u => u.Id == Id);
            if (user != null)
            {
                if (user.IsEnabled)
                    return false;
                return true;
            }
            return false;
        }

        public static bool BlockUser(string phoneNumber)
        {
            var user = Repository.GetContext().Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user != null)
            {
                user.IsEnabled = false;
                Repository.GetContext().Update(user);
                Repository.GetContext().SaveChanges();
                return true;
            }
            return false;

        }

        public static bool UnBlockUser(string phoneNumber)
        {
            var user = Repository.GetContext().Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user != null)
            {
                user.IsEnabled = true;
                Repository.GetContext().Update(user);
                Repository.GetContext().SaveChanges();
                return true;
            }
            return false;

        }
    }
}
