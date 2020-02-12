using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Models;

namespace TelegramBot.BotContext
{
    public class BotApplicationContext:DbContext
    {
        public DbSet<BotUser> Users { get; set; }


        public BotApplicationContext()
        {
            //Database.EnsureCreated();          
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLExpress; Database=TelegramBotDB; Trusted_Connection=True");

        }


    }
}
