using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer.AOEs
{
    public class AOE
    {
        public int id;
        public Components.Transform transform;
        public float alive = 999;
        Service service;
        public AOE(Service service, Vector2 where, Vector2 forward, float timeAlive, ref List<Client> players)
        {
            this.service = service;
            id = service.GetAoeUniqueId();
            Awake();
            transform.setPosition(where);
            List<Vector2> fwds = new List<Vector2>();
            fwds.Add(forward);
            List<float> times = new List<float>();
            times.Add(timeAlive);

            alive = timeAlive;

            transform.setSegment(fwds, times);

            ByteMessage msg = new ByteMessage();
            msg.WriteTag("NEA");
            msg.WriteInt(id);
            msg.WriteFloat(transform.getPosition().X);
            msg.WriteFloat(transform.getPosition().Y);

            foreach(Client c in players)
                c.SendMessage(msg);
        }


        public AOE()
        {
            Awake();
        }

        public virtual void Awake()
        {
            transform = new Components.Transform();
        }

        public virtual void Update(float deltatime, ref List<Client> players, ref List<NPCs.NPC> npcs, ref List<AOE> aoes)
        {
            transform.Update(deltatime);
            alive -= deltatime;
            // Ca nique la boucle for d'update, va falloir trouver une autre strat
            /*
            if (alive < 0)
            {
                aoes.Remove(this);
            }*/
        }
    }
}