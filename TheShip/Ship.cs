using NetworkController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vector;
using System.Runtime.Serialization;
using TheStar;

namespace TheShip
{ 
    /// <summary>
    /// ship" - an int representing the ship's unique ID. Note: ship IDs can be the same as Projectile IDs, 
    /// they are only unique relative to other Ships (same applies for Projectiles and Stars).
    ///
    /// "name" - a string representing the player's name.
    ///
    ///"loc" - a Vector2D representing the ship's location. (See below for description of Vector2D).
    ///
    ///"dir" - a Vector2D representing the ship's orientation.
    ///
    ///"thrust" - a bool representing whether or not the ship was firing engines on that frame.
    ///This can be used by the client to draw a different representation of the ship, e.g., showing engine exhaust.
    ///
    /// deciding to tack on vector 2d For Thrust
    /// 
    /// "hp" - and int representing the hit points of the ship.This value ranges from 0 - 5. 
    /// If it is 0, then this ship is temporarily destroyed, and waiting to respawn. 
    /// If the player controlling this ship disconnects, the server will discontinue sending this ship.
    ///
    /// "score" - an int representing the ship's score.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Ship
    {
        
        //Json Propertiers for the TheShip
        [JsonProperty(PropertyName = "ship")]
        private int shipId;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "dir")]
        private Vector2D dir;
        [JsonProperty(PropertyName = "thrust")]
        private bool thrust;
        [JsonProperty(PropertyName = "name")]
        private string shipName;
        [JsonProperty(PropertyName = "hp")]
        private int hp;
        [JsonProperty(PropertyName = "score")]
        private int score;
        private bool active;
        private double EngineStrength = 0.08;
        private Vector2D velocity = new Vector2D(0,0);
        private bool FireFlag = false;
        private int FireRate;
        private bool right = false;
        private bool left = false;
        private int TimeFired;
        private Vector2D Orientation;
        private Vector2D acceleratoin;
        private int deadTimer;
        private int shotCounter;
        private int hitcounter;

        //Note all three classes need a default constructor in order for the Json Library to work
        //create a new ship empty constructor ship
        public Ship()
        {
            shipId = 0;

            loc = new Vector2D(0.0, 0.0);
            dir = new Vector2D(0.0, -1.0);
            thrust = false;
            shipName = "shipGeneric";
            hp = 5;
            score = 0;
            this.active = true;

        }

        public Ship(int ID, Vector2D loco, Vector2D dir, bool thrusting, string name, int hp, int score, int fire,int dead)
        {
            this.shipId = ID;
            this.shipName = name;
            this.loc = new Vector2D(loco);
            this.dir = new Vector2D(dir);
            this.hp = 5;
            this.score = 0;
            this.active = true;
            this.thrust = thrusting;
            this.FireRate = fire;
            this.deadTimer = dead;
        }
        public void SetShipID(int id)
        {
            this.shipId = id;
        }
        public void setDeadTime(int dead)
        {
            this.deadTimer = dead;
        }
        public void SetVelocity()
        {
            this.velocity = new Vector2D(0,0);
        }
        public void SetShipName(string name)
        {
            this.shipName = name;
        }
        public void SetShipLocation(double x, double y)
        {
            Vector2D location = new Vector2D(x, y);
            this.loc = (location);           
        }
        public void SetShipActive(bool dead)
        {
            this.active = dead;
        }
        public void SetShipHP(int dead)
        {
            this.hp = dead;
        }
        public void SetShipDirection(double x, double y)
        {
            Vector2D direction = new Vector2D(x, y);
            this.dir = (direction);

        }
        public void setFramesPerShot(int framesPerShot)
        {
            FireRate = framesPerShot;
        }
        public bool GetActive()
        {
            if (hp == 0)
            {
                this.active = false;
                return this.active;
            }
            else
            {
                return this.active;
            }                    
        }
        public void HitCount()
        {
            this.hitcounter++;
        }
        public double shotPercentage()
        {
            if(this.shotCounter == 0)
            {
                return 100.0;
            }
            else
            {
                double percentage = this.hitcounter / this.shotCounter;
                return percentage * 100;
            }

        }
       
        public int GetDeadTimer()
        {
            return deadTimer;
        }
        public bool GetAccel()
        {
            return this.thrust;
        }
        public int GetHP()
        {
            return this.hp;
        }
        public int GetID()
        {
            return this.shipId;
        }
        public string GetName()
        {
            return shipName;
        }
        public Vector2D GetLocation()
        {
            return this.loc;
        }
        public Vector2D GetOrientation()
        {
            return this.dir;
        }
        public Vector2D GetEngineThrust()
        {
            if (thrust)
            {
                Vector2D t = new Vector2D(this.dir);
                t = t * EngineStrength;
                return t;
            }
            return new Vector2D(0, 0);
        }
        public int GetScore()
        {
            return score;
        }
        public bool Hit()
        {
            this.hp--;
            hitcounter++;
            if (this.hp == 0)
                return true;
            else
                return false;
        }
        public bool Alive()
        {
            if (hp == 0)
            {
                return false;
            }
            return true;
        }
     


        //see if the sip collieds with the star gets the length of the vector from its
        //current location then see if it is greater than 30 star size
        public bool StarCollider(Star s, Ship ship)
        {        
            if ((ship.GetLocation() - s.GetLocation()).Length() > 35)          
                return true;         
            else
                return false;
        }
        public void Clean()
        {
            this.FireFlag = false;
            this.thrust = false;
            this.right = false;
            this.left = false;
        }

        private void commands()
        {
            if (this.Alive())
            {
                if (this.right == true)
                    this.TurnRight();
                if (this.left == true)
                    this.TurnLeft();
                if (this.FireFlag == true)
                    this.IsFiring();
                if (this.thrust == true)                
                    this.GetEngineThrust();
                
            }
        }

        public bool Pew(int i)
        {
            this.shotCounter++;
            if (!this.Alive() || i - this.TimeFired < this.FireRate)
                return false;
            this.TimeFired = i;
            return true;
        }

        public bool Update(Dictionary<int,Star> star , Ship ship)
        {
            commands();
            //update location based on star the star pull 
            foreach (Star s in star.Values)
            {
                Vector2D g = s.GetLocation() - ship.GetLocation();

                g.Normalize();

                g = g * s.GetMass(); // position of ship relative to star also known as GRAVITY

                this.Orientation = this.GetOrientation();// ships current orientation for accelration on slides

                this.acceleratoin = g + this.GetEngineThrust(); // ships current acceleration

                this.velocity = this.velocity + this.acceleratoin;

                this.loc = this.loc + this.velocity;     
                
                if (StarCollider(s, ship))
                {
                    return true;                   
                }
                else
                {
                    return  false;                 
                }
            }
            return true;
        }

        public void FlagThrust()
        {
            this.thrust = true;
        }

        public void FlagLeft()
        {
            this.left = true;
        }

        public void FlagRight()
        {
            this.right = true;
        }

        public void FlagShoot()
        {
            this.FireFlag= true;
        }

     
        public void TurnLeft()
        {
            Vector2D temp = this.dir;
            temp.Rotate(-2);
            this.dir = temp;
        }
        public void TurnRight()
        {
            Vector2D temp = this.dir;
            temp.Rotate(2);
            this.dir = temp;
        }
        public void Thrust()
        {
            thrust = true;
            this.thrust = true;
        }
        public void NotThrusting()
        {
            thrust = false;
            this.GetEngineThrust();
            this.thrust = false;
        }
        public bool IsFiring()
        {
            return FireFlag;
        }
        public void Point()
        {
            this.score++;
        }
    }
}
