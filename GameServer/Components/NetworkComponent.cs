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
        protected string valueName(string name)
        {
            return "\"" + name + "\"" + ": ";
        }
        protected string valueToJsonFormat(string name, string value)
        {
            return valueName(name) + "\""+value +"\"" + "\n";
        }

        protected string valueToJsonFormat(string name, int value)
        {
            return valueName(name) + value + "\n";
        }

        protected string valueToJsonFormat(string name, float value)
        {
            return valueName(name) + value + "\n";
        }
    }





}
