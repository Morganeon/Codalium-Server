using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer.NPCs
{
    public class NPC
    {
        public int id;
        public Components.Transform transform;
        protected Service service;

        
        public NPC(GameServer.Service service)
        {
            this.service = service;
            id = service.GetNpcUniqueId();
            Awake();
        }

        public virtual void Awake()
        {
            transform = new Components.Transform();
        }

        public virtual void Update(float deltatime, ref List<Client> players, ref List<NPC> npcs, ref List<AOEs.AOE> aoes)
        {
            transform.Update(deltatime);
        }

    }
}
