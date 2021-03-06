﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Actions
{
    public class MoveAction : Action
    {
        public Client c;
        public List<float> dx;
        public List<float> dy;
        public List<float> rx;
        public List<float> ry;
        public List<float> time;


        public MoveAction(List<float> dx, List<float> dy, List<float> time)
        {

            this.dx = dx;
            this.dy = dy;
            for (int i=0;i< dx.Count;i++)
            {
                if (float.IsNaN(dx[i]) || float.IsInfinity(dx[i])) dx[i] = 0;
                if (float.IsNaN(dy[i]) || float.IsInfinity(dy[i])) dy[i] = 0;
                if (float.IsNaN(rx[i]) || float.IsInfinity(rx[i])) rx[i] = 0;
                if (float.IsNaN(ry[i]) || float.IsInfinity(ry[i])) ry[i] = 0;
                if (float.IsNaN(time[i]) || float.IsInfinity(time[i]) || time[i] <= 0) time[i] = 0;
                float magnitude = (float)Math.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);
                if (magnitude!=0)
                {
                    this.dx[i] /= magnitude;
                    this.dy[i] /= magnitude;
                }
                
            }

            this.rx = this.dx;
            this.ry = this.dy;
                this.time = time;
        }

        public MoveAction(List<float> dx, List<float> dy, List<float> rx, List<float> ry, List<float> time)
        {

            this.dx = dx;
            this.dy = dy;
            
            for (int i = 0; i < dx.Count; i++)
            {
                if (float.IsNaN(dx[i]) || float.IsInfinity(dx[i])) dx[i] = 0;
                if (float.IsNaN(dy[i]) || float.IsInfinity(dy[i])) dy[i] = 0;
                if (float.IsNaN(rx[i]) || float.IsInfinity(rx[i])) rx[i] = 0;
                if (float.IsNaN(ry[i]) || float.IsInfinity(ry[i])) ry[i] = 0;
                if (float.IsNaN(time[i]) || float.IsInfinity(time[i]) || time[i] <=0) time[i] = 0;

                float magnitude = (float)Math.Sqrt(dx[i] * dx[i] + dy[i] * dy[i]);
                if (magnitude != 0)
                {
                    this.dx[i] /= magnitude;
                    this.dy[i] /= magnitude;
                }

            }

            this.rx = rx;
            this.ry = ry;
            this.time = time;
        }


        public override void Execute(Client c)
        {
            List<Vector2> moves = new List<Vector2>();
            for (int i = 0; i < dx.Count; i++)
            {
                moves.Add(new Vector2(dx[i], dy[i]));
            }
            c.transform.setSegment(moves,time);
        }
    }
}
