using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.AOEs;

namespace GameServer.NPCs
{
    public class NPCVner : NPC
    {
        float cooldown = 0.1f;
        Client target;
        public NPCVner(Service service) : base(service)
        {
            //hello
        }


        public override void Update(float deltatime, ref List<Client> players, ref List<NPC> npcs, ref List<AOEs.AOE> aoes)
        {
           transform.Update(deltatime);
           cooldown -= deltatime;



           if (players.Count > 0)
           {
               // acquire target
               if (target == null)
               {
                   float mindistance = 99999;
                   foreach (Client c in players)
                   {
                       float distance = Vector2.Distance(c.transform.getPosition(), transform.getPosition());
                       if (distance < mindistance)
                       {
                           mindistance = distance;
                           target = c;
                       }
                   }
               }


               if (target != null)
               {
                    // bring the hell out of him
                    if (cooldown <= 0)
                    {
                        cooldown = 0.1f;
                        aoes.Add(new AOEs.AOE(service, transform.getPosition(), target.transform.getPosition() - transform.getPosition(), 3, ref players));
                    }
               }
               
           }

        }

    }
}
