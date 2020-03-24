using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer.Components
{
    class Transform
    {

        public Transform()
        {

        }


        void update(float dt)
        {
            //Normalement pas bugué ^:)^
            float remaining_time = dt;
            //TODO add speed factor (altered by velocity heading difference)
           
            while (remaining_time > 0) {
                if (remaining_time > times[0])
                {
                    remaining_time -= times[0];
                    pos += velocities[0] * times[0] * speed;
                    times.RemoveAt(0);
                    velocities.RemoveAt(0);
                    if (times.Count == 0)
                    {
                        break;
                    }
                }
                else
                {
                    times[0] -= remaining_time;
                    pos += velocities[0] * remaining_time * speed;
                    if (times[0] < epsilon)
                    {
                        times.RemoveAt(0);
                        velocities.RemoveAt(0);
                    }
                    break;
                }
            }

        }
        public void setHeading(Vector2 heading_i){
            heading = heading_i;
        }

        Vector2 pos;
        Vector2 heading;

        List<float> times; // [s]
        List<Vector2> velocities;

        float epsilon = 0.000001f;
        float speed = 1.0f;//GL
    }
}
