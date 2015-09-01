using dokkasz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokkasz.ViewModels
{
    public class EgysegTorzsWindowViewModel
    {
        private readonly TorzsViewModel<EgysegViewModel> viewModel;

        public TorzsViewModel<EgysegViewModel> ViewModel
        {
            get { return viewModel; }
        }

        public EgysegTorzsWindowViewModel()
        {
            viewModel = new TorzsViewModel<EgysegViewModel>(c => c.EgysegTorzs, e => new EgysegViewModel((Egyseg)e));
        }
    }
}
