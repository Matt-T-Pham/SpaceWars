using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector;

namespace TheStar
{
    /// <summary>
    /// "Star" - an int representing the Star's unique ID.
    /// 
    ///"loc" - a Vector2D representing the Star's location.
    ///
    ///"mass" - a double representing the Star's mass. Note that the sample client does not use this information, 
    ///but you may choose to display stars differently based on their mass
    ///
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Star
    {
        [JsonProperty(PropertyName = "star")]
        private int ID;
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;
        [JsonProperty(PropertyName = "mass")]
        private double mass;       
        
        //Note all three classes need a default constructor in order for the Json Library to work

        //empty argument Constructor
        public Star()
        {
            this.ID = 0;
            this.loc = new Vector2D(0.0 , 0.0);
            this.mass = 0.0;
        }
        public Star(int id, Vector2D loco, double m)
        {
            this.ID = id;
            this.loc = loco;
            this.mass = m;
        }
        public int GetID()
        {
            return this.ID;
        }
        public double GetMass()
        {
            return this.mass;
        }
        public Vector2D GetLocation()
        {
            return this.loc;
        }
        public bool GetActive()
        {
            return true;
        }
    }
}
