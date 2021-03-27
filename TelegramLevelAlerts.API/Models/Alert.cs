using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramLevelAlerts.API.Models
{
    public class Alert
    {
        /// <summary>
        /// GUID
        /// </summary>
        public Guid Id { get; set; }
        public string Message { get; set; }
        
        public override string ToString() => GetMessage();

        #region Private Methods
        string GetMessage()
        {
            string msg = "";

            if (!string.IsNullOrEmpty(Message))
                msg = $"*Mensagem:* _{Message}_";
            
            return msg;
        }
        #endregion
    }
}