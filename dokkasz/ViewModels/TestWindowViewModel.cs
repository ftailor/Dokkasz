using dokkasz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dokkasz.ViewModels
{
    class TestWindowViewModel
    {
        private readonly TorzsViewModel<TesztViewModel> viewModel;

        public TorzsViewModel<TesztViewModel> ViewModel
        {
            get { return viewModel; }
        }

        public TestWindowViewModel()
        {
            viewModel = new TorzsViewModel<TesztViewModel>(c => c.TesztTorzs, t => new TesztViewModel((Teszt)t));
        }
    }
}
