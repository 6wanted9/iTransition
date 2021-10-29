using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace task4.Models
{
    public static class BroAndSis
    {
        private static string bro;
        private static string sis;
        private static string lastMessage;
        private static string lastUser;
        private static string timeOfLastMsg;
        public static string Bro
        {
            get
            {
                return bro;
            }
        }
        public static string Sis
        {
            get
            {
                return sis;
            }
        }
        public static string LastMessage
        {
            get
            {
                return lastMessage;
            }
        }
        public static string LastUser
        {
            get
            {
                return lastUser;
            }
        }
        public static string TimeOfLastMsg
        {
            get
            {
                return timeOfLastMsg;
            }
        }

        static BroAndSis()
        {
            bro = "0";
            sis = "0";
        }
        public static void BroClick(string User)
        {
            bro = Convert.ToString(Convert.ToInt32(bro) + 1);
            lastMessage = "Bro!";
            timeOfLastMsg = DateTime.UtcNow.ToLocalTime().ToShortTimeString();
            lastUser = User;
        }
        public static void SisClick(string User)
        {
            sis = Convert.ToString(Convert.ToInt32(sis) + 1);
            lastMessage = "Sis!";
            timeOfLastMsg = DateTime.UtcNow.ToLocalTime().ToShortTimeString();
            lastUser = User;
        }
    }
}
