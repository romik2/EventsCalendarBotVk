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
        public static void UpdateBdEvents(string id_events)
        {
            MySqlConnection conn = new MySqlConnection(Config.mysql);
            conn.Open();
            MySqlCommand command = new MySqlCommand("UPDATE `EventsCalendar`.`events` SET `notification` = '0' WHERE (`id` = '"+ id_events + "');", conn);
            command.ExecuteNonQuery();
            conn.Close();
        }
        public static void messagesSend()
        {
            string name_events = " ";
            string provisions_events = " ";
            string date_events = " ";
            MySqlConnection conn = new MySqlConnection(Config.mysql);
            conn.Open();
            MySqlCommand command_events = new MySqlCommand("SELECT min(id) FROM events WHERE Notification = 1", conn);
            string id_events = Convert.ToString(command_events.ExecuteScalar());
            command_events = new MySqlCommand("SELECT id, name, provisions, DATE_FORMAT(date,'%d.%m.%Y') FROM events WHERE id = " + id_events, conn);
            MySqlDataReader reader_events = command_events.ExecuteReader();
            while (reader_events.Read())
            {
                id_events = reader_events[0].ToString();
                name_events = reader_events[1].ToString();
                provisions_events = reader_events[2].ToString();
                date_events = reader_events[3].ToString();
            }
            reader_events.Close();
            MySqlCommand command_users = new MySqlCommand("SELECT link_vk FROM users", conn);
            MySqlDataReader reader_users = command_users.ExecuteReader();
            while (reader_users.Read())
            {
                api.Messages.Send(new MessagesSendParams()
                {
                    UserId = (long)Convert.ToInt32(reader_users[0]),
                    Message = "Спотик: Появилось новое мероприятие " + name_events + " в направлении " + provisions_events + ". Последняя дата подачи документов " + date_events + ".",
                    RandomId = new Random().Next()
                });
            }
            reader_users.Close(); 
            conn.Close();
            UpdateBdEvents(id_events);
        }
        public static bool ProverkaEvent()
        {
            MySqlConnection conn = new MySqlConnection(Config.mysql);
            conn.Open();
            string sql = "SELECT count(id) FROM events WHERE Notification = 1";
            MySqlCommand command = new MySqlCommand(sql, conn);
            if (Convert.ToInt32(command.ExecuteScalar()) > 0)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
            
        }
        public static void Auth()
        {
            ApiAuthParams apis = new ApiAuthParams()
            {
                AccessToken = Config.token,
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
            while (true)
            {
                Auth();
                if (ProverkaEvent() == true)
                {
                    messagesSend();
                }
                else
                {
                    Console.WriteLine("Мероприятий нет в базе данных");
                }
            }    
        }
    }
}
