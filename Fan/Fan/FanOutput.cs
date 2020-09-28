using System;
using System.Linq.Expressions;

namespace Fan
{
    public class FanOutput
    {
        //private int _id;
        private string _name;
        private int _temp;
        private int _fugt;


        public FanOutput() { }

        public FanOutput(int id, string name, int temp, int fugt)
        {
            CheckName(name);
            CheckTemp(temp);
            CheckFugt(fugt);
            Id = id;
            Name = name;
            Temp = temp;
            Fugt = fugt;
        }

        public int Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                CheckName(value); _name = value;
            }
        }

        public int Temp
        {
            get => _temp;
            set
            {
                CheckTemp(value); _temp = value;
            }
        }

        public int Fugt
        {
            get => _fugt;
            set
            {
                CheckFugt(value); _fugt = value;
            }
        }

        public static void CheckName(string name)
        { 
            if(name.Length < 2){
                throw new Exception("Name should be at least 2 characters");
            }
        }

        public static void CheckTemp(int temp)
        {
            if (15 > temp || temp > 25)
            {
                throw new Exception("Temperature should be between 15 and 25");
            }
        }

        public static void CheckFugt(int fugt)
        {
            if (30 > fugt || fugt > 80)
            {
                throw new Exception("Humidity should be between 30 and 80");
            }
        }

        public override string ToString()
        {
            return $"Fan id = {Id}, fan name = {Name}, the temperature is = {Temp} and the humidity is = {Fugt}";
        }
    }
}
