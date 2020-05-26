using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projectile;
using Server;
using System;
using System.Collections.Generic;
using TheShip;
using TheStar;
using TheWorld;
using Vector;
namespace ServerTest
{
    [TestClass]
    public class UnitTest1
    {
        int UniverseSize = 750;
        int StarID = 0;
        int ShipID = 0;

        public Dictionary<int, Ship> ship = new Dictionary<int, Ship>();
        public Dictionary<int, Proj> proj = new Dictionary<int, Proj>();
        public Dictionary<int, Star> star = new Dictionary<int, Star>();

        int FramesPerShot = 15;
        Random rand = new Random();
        int respawn = 300;

        public const string Alphabet =
        "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateName()
        {
            char[] chars = new char[5];
            for (int i = 0; i < 5; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }

        private int AssighnShipID()
        {
            return ShipID++;
        }
        public Vector2D randomVector()
        {
            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);
            Vector2D t = new Vector2D(j, k);
            return t;
        }
        public Ship RandomShipGen()
        {

            Ship s = new Ship();
            s.SetShipID(AssighnShipID());
            s.SetShipName(GenerateName());
            s.setFramesPerShot(FramesPerShot);
            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);

            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            s.SetShipLocation(j, k);

            return s;
        }
        private int AssighnStarID()
        {
           return StarID++;
        }
        public Star randomStar()
        {
            Star s = new Star(AssighnStarID(),randomVector(),11);
            return s;
        }


























        [TestMethod]
        public void StarGetID()
        {           
            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            double mass = .11;          

            Star s = new TheStar.Star(AssighnStarID(),test,mass);

            Assert.AreEqual(s.GetID(), 0);            
        }
        [TestMethod]
        public void StarGetMass()
        {

            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            double mass = .11;

            Star s = new TheStar.Star(AssighnStarID(), test, mass);

            Assert.AreEqual(s.GetMass(), 0.11);
        }
        [TestMethod]
        public void StarGetLocoationX()
        {

            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            double mass = .11;

            Star s = new TheStar.Star(AssighnStarID(), test, mass);

            Assert.AreEqual(s.GetLocation().GetX(), j);
        }
        [TestMethod]
        public void StarGetLocoationY()
        {

            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            double mass = .11;

            Star s = new TheStar.Star(AssighnStarID(), test, mass);

            Assert.AreEqual(s.GetLocation().GetY(), k);
        }
        [TestMethod]
        public void StarGetActive()
        {

            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);
            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            double mass = .11;

            Star s = new TheStar.Star(AssighnStarID(), test, mass);

            Assert.AreEqual(s.GetActive(), true);
        }
        [TestMethod]
        public void SetShipFunction()
        {

            Random random = new Random();

            double j = random.Next(-UniverseSize / 2, UniverseSize / 2);

            double k = random.Next(-UniverseSize / 2, UniverseSize / 2);

            Vector2D test = new Vector2D(j, k);

            Ship s = new Ship();

            s.SetShipActive(true);
            Assert.AreEqual(s.GetActive(), true);

            s.SetShipDirection(j,k);
            Assert.AreEqual(s.GetOrientation(), test);

            s.SetShipHP(5);
            Assert.AreEqual(s.GetHP(),5);

            s.SetShipID(10);
            Assert.AreEqual(s.GetID(), 10);

            s.SetShipLocation(j,k);
            Assert.AreEqual(s.GetLocation(), test);

            s.SetShipName("Test");
            Assert.AreEqual(s.GetName(), "Test");

            Assert.AreEqual(s.GetScore(), 0);

            Assert.AreEqual(s.GetAccel(), false);

            s.SetVelocity();

            s.setFramesPerShot(15);
            
        }
        [TestMethod]
        public void ShipActive()
        {
            Ship s = new Ship();

            while (s.GetHP() > 0) 
            {
                s.Hit();
            }

            Assert.AreEqual(s.GetActive(), false);

        }
        [TestMethod]
        public void ShipAlive()
        {
            Ship s = new Ship();

            while (s.Alive() == true)
            {
                s.Hit();
            }
            Assert.AreEqual(s.Alive(), false);

        }

        [TestMethod]
        public void ShipFlagUpdate()
        {
            Ship s = RandomShipGen();

            Star st = randomStar();

            star.Add(st.GetID(),st);

            s.FlagThrust();
            s.FlagShoot();
            s.FlagRight();
            s.FlagLeft();
            s.IsFiring();

            s.Update(star,s);

        }

        [TestMethod]
        public void WorldDefaultConstructor()
        {
            World w = new World();
            Assert.AreEqual(w.WorldSize, 750);
        }

        [TestMethod]
        public void WorldShipsCount()
        {
            World w = new World();
            Ship s = RandomShipGen();
            w.Update();
            Dictionary<int, Ship> wShips = new Dictionary<int, Ship>();
            wShips = w.GetShips();
            Assert.AreEqual(wShips.Count, w.GetShips().Count);
        }

        [TestMethod]
        public void Respawn()
        {
            World w = new World();
            Ship s = RandomShipGen();
            w.Update();
            Dictionary<int, Ship> wShips = new Dictionary<int, Ship>();
            wShips = w.GetShips();
            Assert.AreEqual(wShips.Count, w.GetShips().Count);
        }

        [TestMethod]
        public void WorldSet()
        {
            World w = new World();
            w.SetFireRate(FramesPerShot);
            w.SetRespawn(respawn);
            w.SetWorldSize(UniverseSize);
        }

        [TestMethod]
        public void TestShoot()
        {
            World w = new World();
            for(int i =0;i<10; i++)
            {
                Ship s = RandomShipGen();
                w.GetShips().Add(s.GetID(), s);
            }
    
            foreach(Ship st in w.GetShips().Values)
            {
                st.FlagShoot();
            }
            w.Update();
           
        }

        [TestMethod]
        public void TestDie()
        {
            World w = new World();
            List<Ship> sr = new List<Ship>();

            for (int i = 0; i < 10; i++)
            {
                Ship s = RandomShipGen();
                w.GetShips().Add(s.GetID(), s);
            }

            foreach (Ship st in w.GetShips().Values)
            {
                st.SetShipActive(false);
            }
            w.Update();

        }
    }
}
