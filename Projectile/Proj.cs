using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStar;
using Vector;
using TheShip;

namespace Projectile { 

    /// <summary>
    /// "proj" - an int representing the projectile's unique ID.
    /// 
    ///"loc" - a Vector2D representing the projectile's location.
    ///
    ///"dir" - a Vector2D representing the projectile's orientation.
    ///
    ///"alive" - a bool representing if the projectile is active or not.
    ///The server will send the deactivated projectiles only once.
    ///
    ///"owner" - an int representing the ID of the ship that created the projectile
    ///You can use this to draw the projectiles with a different color or image.
    /// </summary>
    /// 
    [JsonObject(MemberSerialization.OptIn)]
    public class Proj
    {
        [JsonProperty(PropertyName = "proj")]
        private int ID;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "dir")]
        private Vector2D dir;
        [JsonProperty(PropertyName = "alive")]
        private bool alive;
        [JsonProperty(PropertyName = "owner")]
        private int owner;


        //Note all three classes need a default constructor in order for the Json Library to work
        //empty argument constructor
        public Proj()
        {
            ID = 0;
            loc = new Vector2D(0.0, 0.0);
            dir = new Vector2D(0.0, 0.0);
            alive = true;
            owner = 0;
        }
        //multiargument constructor
   
        public Proj(int id, Vector2D loco, Vector2D direc,int own)
        {
            this.ID = id;
            this.owner = own;
            this.loc = new Vector2D(loco);
            this.dir = new Vector2D(direc);
            this.alive = true;   
        }
        public void SetActive(bool act)
        {
            this.alive = act;
        }
        public Vector2D GetLocation()
        {
            return this.loc;
        }

        public Vector2D GetOrientation()
        {
            return this.dir;
        }
        public int GetID()
        {
            return this.ID;
        }
        public int GetOwner()
        {
            return this.owner;
        }

        public bool GetActive()
        {
            return this.alive;
        }
        public bool StarCollider(Star s , Proj p)
        {
                if ((p.GetLocation() - s.GetLocation()).Length() > 35)
                {
                    return true;
                }
                else
                    return false;           
        }
        public bool Update(Dictionary<int, Star> star,Dictionary<int,Ship> ships)
        {
            // calculate Position

            this.loc = this.loc + this.dir * 15.0;

            foreach (Star s in star.Values)
            {
                if (!StarCollider(s, this))
                {
                    this.alive = false;
                    return false;
                }
            }
            return true;
        }
    }
}
