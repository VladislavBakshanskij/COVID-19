using System;

namespace Covid_19 {
    public class Coronavirus {
        public int Cases { get; set; }
        public int Death { get; set; }
        public int Recovered { get; set; }
        public DateTime Date { get; set; }

        public Coronavirus() { 
        }

        public Coronavirus(int cases, int death, int recovered, DateTime date) {
            Cases = cases;
            Death = death;
            Recovered = recovered;
            Date = date;
        }

        public override string ToString() => $"Дата: {Date}\n--Число зараженных: {Cases}\n--Число умерших: {Death}\n--Число выживших: {Recovered}";
    }
}
