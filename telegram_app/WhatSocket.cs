using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_app
{
    class WhatSocket
    {
        private static Telegram.Bot.Api _instance;
        
        public static Telegram.Bot.Api Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                else
                {
                    throw new Exception("Instance is not set");
                }
            }
        }

        public static void Create(string token)
        {
            //_instance = new WhatsAppApi.WhatsApp(username, password, nickname, debug);
            _instance = new Telegram.Bot.Api(token);
        }
    }
}
