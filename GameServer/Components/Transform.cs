using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameServer.Components
{
    public class Transform : NetworkComponent
    {

        float epsilon = 0.000001f;

        Vector2 pos { get; set; }

        Vector2 heading { get; set; }
        float speed { get; set; }



        List<float> times; // [s]

        List<Vector2> velocities;


        public override void Awake()
        {
            pos = new Vector2(0, 0);
            heading = new Vector2(0, 0);
            times = new List<float>();
            velocities = new List<Vector2>();
        }

        public Transform()
        {
            pos = new Vector2(0, 0);
            heading = new Vector2(0, 0);
        }


        public Transform(Vector2 pos_i, Vector2 heading_i, float speed_i)
        {
            pos = new Vector2(pos_i.X, pos.Y);
            heading = new Vector2(heading_i.X,heading_i.Y);
            speed = speed_i;
        }


        public override void Update(float dt)
        {
            //Normalement pas bugué ^:)^
            float remaining_time = dt;
            //TODO add speed factor (altered by velocity heading difference)
            while (remaining_time > 0 && times.Count > 0) {
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

        public void setSegment(List<Vector2> velocities_i, List<float> time_i)
        {
            times = time_i;
            velocities = velocities_i;
        }

        public void setSpeed(float speed_i)
        {
            speed = speed_i;
        }

        public void setPosition(Vector2 pos)
        {
            this.pos = pos;
        }

        public Vector2 getPosition()
        {
            return pos;
        }

        public override string serialize()
        {
            string ser = this.startJsonString();
            ser += this.valueToJsonFormat("px", pos.X.ToString());
            ser += this.valueToJsonFormat("py", pos.Y.ToString());
            ser += this.valueToJsonFormat("rx", heading.X.ToString());
            ser += this.valueToJsonFormat("ry", heading.Y.ToString());
            ser += this.valueToJsonFormat("v", speed.ToString());
            ser += this.endJsonString();
            return ser;
        }



    }
}
