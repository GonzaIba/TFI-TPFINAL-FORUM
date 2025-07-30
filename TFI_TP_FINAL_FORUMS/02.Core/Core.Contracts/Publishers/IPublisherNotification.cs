using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts.Publishers
{
    public interface IPublisherNotification
    {
        Task AddNotification(int codeNotification, string codeUser, string message, DateTime date, bool readed);
        Task RemoveNotification(int codeNotification, string codeUser);
    }
}
