using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvantApp
{
    public partial class Messages
    {
        public string FullInfoUser
        {
            get
            {
                var user = Helper.GetContext().User.Where(p => p.IdUser == MessageUser).ToList();
                string fullInfo;
                fullInfo = user.First().SecondName + " " + user.First().FirstName + " " + user.First().Patronymic + " Тел.-" + user.First().Phone + " Email-" + user.First().Email + " возраст:" + user.First().Age;
                return fullInfo;
            }
        }
    }
    static class Container
    {
        public static int idEvent;
        public static int idStaff;
    }
}
