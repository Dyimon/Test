using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvantApp
{
    internal class Helper
    {
        private static CallPresidentEntities callPresidentEntities;
        public static CallPresidentEntities GetContext()
        {
            if(callPresidentEntities == null)
            {
                callPresidentEntities = new CallPresidentEntities();
            }
            return callPresidentEntities;
        }
    }
}
