using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheShip;
using Projectile;
using TheStar;
using System.Timers;
using System.Diagnostics;
using Vector;
using System.Threading;

namespace TheWorld
{
    public class World
    {
        public Dictionary<int, Ship> ship;
        public Dictionary<int, Proj> proj;
        public Dictionary<int, Star> star;
        public Dictionary<int, Ship> deadShip;
        public Dictionary<int, int> Score;
        public int WorldSize;

        private int worldRespawn;
        private int worldSize;

        private Dictionary<int, Ship> Graveyard;

        private List<Ship> purgatory;
        private static int ProjID = 0;

        private int shootTimer;
        int time;

        public World()
        {
            this.WorldSize = 750;
            ship = new Dictionary<int, Ship>();
            proj = new Dictionary<int, Proj>();
            star = new Dictionary<int, Star>();
            deadShip = new Dictionary<int, Ship>();
            Score = new Dictionary<int, int>();
            purgatory = new List<Ship>();
            Graveyard = new Dictionary<int, Ship>();
        }
        public void SetRespawn(int Respawn)
        {
            this.worldRespawn = Respawn;
        }
        public void SetWorldSize(int worldSize)
        {
            this.worldSize = worldSize;
        }
        public Dictionary<int, Ship> GetDead()
        {
            return this.deadShip;
        }
        public Dictionary<int, Ship> GetGraveyard()
        {
            return this.Graveyard;
        }
        public Dictionary<int, Ship> GetShips()
        {
            return this.ship;
        }
        public Dictionary<int, Proj> GetProj()
        {
            return this.proj;
        }

        public void SetFireRate(int framesPerShot)
        {
            shootTimer = framesPerShot;
        }

        public Dictionary<int, Star> GetStar()
        {
            return this.star;
        }
        private void Die(List<Ship> s)
        {
            lock (purgatory)
            {
                lock (Graveyard)
                {
                    foreach (Ship dead in s.ToList())
                    {
                        dead.SetShipActive(false);
                        dead.SetShipHP(0);
                        Graveyard.Add(time, dead);
                    }
                }
                purgatory.Clear();
            }
        }
        private void respawn1()
        {
            Random random = new Random();


            double j = random.Next(-worldSize / 2, worldSize / 2);
            double k = random.Next(-worldSize / 2, worldSize / 2);

            foreach (KeyValuePair<int, Ship> entry in Graveyard.ToList())
            {
                if (time-entry.Key > worldRespawn)
                {
                    entry.Value.SetShipActive(true);
                    entry.Value.SetShipHP(5);
                    entry.Value.SetShipLocation(j, k);
                    entry.Value.SetVelocity();
                    Graveyard.Remove(entry.Key);
                    ship.Add(entry.Value.GetID(), entry.Value);
                }
            }

        }

        public void Update()
        {
            bool deadFlag = false;

            lock (this)
            {
                // use ship.Update and projectile.Update to calculate state of the world
                foreach (Ship s in ship.Values)
                {
                    if (s.GetActive())
                    {
                        //changed update ship to a bool so that we can implement a respawn mechanism
                        //if the ship is alive update the ship
                        //if the ship should technically be dead then run respawn method
                        if (s.Update(star, s))
                        {
                            if (s.IsFiring())
                            {
                                if (s.Pew(time))
                                {
                                    Proj pro = new Proj(AssignIDProj(), s.GetLocation(), s.GetOrientation(), s.GetID());
                                    proj.Add(pro.GetID(), pro);
                                }
                            }                                                                                            
                            //implement wrap around.
                            if (s.GetLocation().GetX() > WorldSize / 2 || s.GetLocation().GetX() < -WorldSize / 2)
                            {
                                s.SetShipLocation(-s.GetLocation().GetX(), s.GetLocation().GetY());
                            }
                            if (s.GetLocation().GetY() > WorldSize / 2 || s.GetLocation().GetY() < -WorldSize / 2)
                            {
                                s.SetShipLocation(s.GetLocation().GetX(), -s.GetLocation().GetY());
                            }  
                        }
                        else
                        {
                            purgatory.Add(s);
                            deadFlag = true;
                        }
                    }
                }
                if (deadFlag)
                    lock (purgatory)
                    {
                        Die(purgatory);
                    }


                foreach (Proj p in proj.Values)
                    {
                        if (p.GetActive())
                        {
                                 //checks to see if the projectile hits the edge of the world 
                                if (p.Update(star,ship))
                                {
                                    if (p.GetLocation().Length() > Math.Sqrt(2.0 * (WorldSize / 2.0) * (WorldSize / 2.0)))
                                    {
                                        p.SetActive(false);
                                    }
                                    //loop through and checks to see if the projectiles are hitting
                                    foreach (Ship ships in ship.Values)
                                    {                              
                                        if (ships.Alive() && p.GetOwner() != ships.GetID())
                                        {
                                            double collision = (p.GetLocation() - ships.GetLocation()).Length();

                                        //detects collosion
                                            if (collision < 25)
                                            {
                                                p.SetActive(false);
                                                 
                                                ships.Hit();
                                                ship[p.GetOwner()].HitCount();
                                            //kills the ship and increments points
                                                if (ships.Alive()==false)
                                                {
                                                    if (this.ship.ContainsKey(p.GetOwner()))
                                                    {
                                                        ship[p.GetOwner()].Point();
                                                    }
                                                    ships.SetShipActive(false);
                                                    deadFlag = true;
                                                    purgatory.Add(ships);

                                                }
                                            }
                                        }
                                    }
                                }
                        }
                }
            }

            time++;

        }
        private static int AssignIDProj()
        {
            return ProjID++;
        }
        public void CleanUp()
        {        
            respawn1();
            purgatory.Clear();

            foreach (Ship s in ship.Values.ToList())
            {
                if (!s.GetActive())
                {
                    ship.Remove(s.GetID());

                }
                s.Clean();
            }
            foreach (Proj p in proj.Values.ToList())
            {
                    if (p.GetActive() == false)
                    {
                       proj.Remove(p.GetID());
                    }
            }
        }
            
    }
}
