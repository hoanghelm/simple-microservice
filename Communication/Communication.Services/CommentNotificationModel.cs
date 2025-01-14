using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Services
{
    public class CommentNotificationModel
    {
        public Guid PostId { get; set; }
        public string AuthorId { get; set; }
        public string Email { get; set; }
        public string AuthorName { get; set; }
        public ComunicationStatus NotificationStatus { get; set; }
    }
}
