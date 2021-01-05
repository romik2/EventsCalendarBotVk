using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//VkNet
using VkNet;
using VkNet.Model.RequestParams;
using VkNet.Model.Keyboard;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Enums.SafetyEnums;
//MySql
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace EventsCalendarBotVk
{
    class Program
    {
        public static VkApi api = new VkApi();
        public static void Auth()
        {
            ApiAuthParams apis = new ApiAuthParams()
            {
                AccessToken = "",
                Settings = Settings.All
            };

            try
            {
                api.Authorize(apis);
                if (api.IsAuthorized)
                {
                    Console.WriteLine("Соединение с API установлено!" + "\r");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: Соединение не установлено, обратитесь ктеническому администратору!" + ex + "\r");
            }
        }
        static void Main(string[] args)
        {
            Auth();
            Console.ReadKey();
        }
    }
}
