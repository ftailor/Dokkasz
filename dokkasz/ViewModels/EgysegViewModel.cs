using dokkasz.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokkasz.ViewModels
{
    public class EgysegViewModel : EntityViewModel
    {
        private readonly Egyseg egyseg;

        protected override object Entity
        {
            get { return egyseg; }
        }

        [DisplayName("Kód")]
        [Required(ErrorMessage = "A Kód nem lehet üres!")]
        public string Kod
        {
            get { return egyseg.Kod?.Trim(); }
            set
            {
                egyseg.Kod = !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;
                OnPropertyChanged();
            }
        }

        [DisplayName("Megnevezés")]
        [Required(ErrorMessage = "A Megnevezés nem lehet üres!")]
        public string Megnevezes
        {
            get { return egyseg.Megnevezes?.Trim(); }
            set
            {
                egyseg.Megnevezes = !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;
                OnPropertyChanged();
            }
        }

        [DisplayName("Áfa?")]
        public bool Afa
        {
            get { return egyseg.Afa; }
            set
            {
                egyseg.Afa = value;
                OnPropertyChanged();
            }
        }

        public EgysegViewModel() : this(new Egyseg())
        {
        }

        public EgysegViewModel(Egyseg egyseg)
        {
            this.egyseg = egyseg;
        }
    }
}
