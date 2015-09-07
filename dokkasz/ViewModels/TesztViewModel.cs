using dokkasz.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokkasz.ViewModels
{
    class TesztViewModel : EntityViewModel
    {
        private readonly Teszt teszt;

        protected override object Entity
        {
            get { return teszt; }
        }

        [DisplayName("Név")]
        [Required(ErrorMessage = "A Név nem lehet üres!")]
        [MaxLength(50, ErrorMessage = "A Név maximum 50 karakter hosszú lehet!")]
        public string Nev
        {
            get { return teszt.Nev; }
            set
            {
                teszt.Nev = value;
                OnPropertyChanged();
            }
        }

        [DisplayName("Fizetés")]
        public int? Fizetes
        {
            get { return teszt.Fizetes; }
            set
            {
                teszt.Fizetes = value;
                OnPropertyChanged();
            }
        }

        [DisplayName("Dátum")]
        public string Datum
        {
            get { return teszt.Datum?.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture); }
            set
            {
                teszt.Datum = DateTime.ParseExact(value, "yyyy.MM.dd", CultureInfo.InvariantCulture);
                OnPropertyChanged();
            }
        }

        public TesztViewModel() : this(new Teszt())
        {
        }

        public TesztViewModel(Teszt teszt)
        {
            this.teszt = teszt;
        }
    }
}
