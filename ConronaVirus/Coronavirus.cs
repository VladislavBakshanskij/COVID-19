using System;

namespace CoronaVirus {
    public class Coronavirus {
        public int Cases { get; set; }
        public int Death { get; set; }
        public int Recovered { get; set; }
        public DateTime Date { get; set; }

        public Coronavirus() { 
        }

        public Coronavirus(int cases, int death, int recovered) {
            this.Cases = cases;
            this.Death = death;
            this.Recovered = recovered;
            Date = DateTime.Now;
        }

        public override string ToString() => $"Дата: {this.Date}\n--Число зараженных: {this.Cases}\n--Число умерших: {this.Death}\n--Число выживших: {this.Recovered}";
    }
}
