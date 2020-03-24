using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameServer.Components
{
    public class NetworkComponent : Component
    {
        public virtual string serialize()
        {
            return "";
        }


        protected string startJsonString()
        {
            return "{\n";
        }

        protected string endJsonString()
        {
            return "}";
        }
        protected string valueToJsonFormat(string name, string value)
        {
            return name + ":" + value +"\n";
        }



    }





}
