using dokkasz.Model;
using dokkasz.Utilities;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace dokkasz.ViewModels
{
    public class TorzsViewModel<TViewModel> : BindableBase
        where TViewModel : EntityViewModel
    {
        private readonly Func<DokkaszEntities, IQueryable> query;
        private readonly Func<object, TViewModel> wrap;
        private readonly ObservableCollection<PropertyAdapter<TViewModel>> properties;
        private readonly ListCollectionView propertiesView;
        private readonly ObservableCollection<TViewModel> items;
        private readonly ListCollectionView itemsView;
        private DokkaszEntities context;
        private readonly DelegateCommand loadCommand;
        private readonly DelegateCommand commitCommand;
        private readonly DelegateCommand cancelCommand;
        private readonly DelegateCommand reloadCommand;
        //private readonly DelegateCommand editCommand;
        private readonly DelegateCommand toggleEditCommand;
        //private readonly DelegateCommand<object> saveCommand;
        private readonly DelegateCommand<object> findCommand;
        private bool isInEditMode;
        private bool isLoading;
        //private bool isSaving;

        public ListCollectionView PropertiesView
        {
            get { return propertiesView; }
        }

        public ListCollectionView ItemsView
        {
            get { return itemsView; }
        }

        public DelegateCommand LoadCommand
        {
            get { return loadCommand; }
        }

        public DelegateCommand CommitCommand
        {
            get { return commitCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return cancelCommand; }
        }

        public DelegateCommand ReloadCommand
        {
            get { return reloadCommand; }
        }

        /*
        public DelegateCommand EditCommand
        {
            get { return editCommand; }
        }
        */

        public DelegateCommand ToggleEditCommand
        {
            get { return toggleEditCommand; }
        }

        /*
        public DelegateCommand<object> SaveCommand
        {
            get { return saveCommand; }
        }
        */

        public DelegateCommand<object> FindCommand
        {
            get { return findCommand; }
        }

        public bool IsInEditMode
        {
            get { return isInEditMode; }
            private set
            {
                SetProperty(ref isInEditMode, value);
                //ReloadCommand.RaiseCanExecuteChanged();
                //EditCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditingItem
        {
            get { return ItemsView.IsEditingItem || ItemsView.IsAddingNew; }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set
            {
                SetProperty(ref isLoading, value);
                CommitCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
                ReloadCommand.RaiseCanExecuteChanged();
                //EditCommand.RaiseCanExecuteChanged();
                ToggleEditCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSaving
        {
            get { return false; }
            //get { return isSaving; }
            //private set { SetProperty(ref isSaving, value); }
        }

        public TorzsViewModel(Func<DokkaszEntities, IQueryable> query, Func<object, TViewModel> wrap)
        {
            this.query = query;
            this.wrap = wrap;
            this.properties = new ObservableCollection<PropertyAdapter<TViewModel>>();
            propertiesView = new ListCollectionView(this.properties);

            var properties = typeof(TViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var property in properties)
            {
                this.properties.Add(new PropertyAdapter<TViewModel>(property));
            }

            items = new ObservableCollection<TViewModel>();
            items.CollectionChanged += ItemsChanged;
            itemsView = new ListCollectionView(items);
            (ItemsView as INotifyPropertyChanged).PropertyChanged += ItemsView_PropertyChanged;
            context = new DokkaszEntities();
            loadCommand = DelegateCommand.FromAsyncHandler(LoadAsync);
            commitCommand = new DelegateCommand(CommitEdit, () => IsEditingItem && !IsLoading);
            cancelCommand = new DelegateCommand(CancelEdit, () => IsEditingItem && !IsLoading);
            reloadCommand = DelegateCommand.FromAsyncHandler(ReloadAsync, () => !IsEditingItem && !IsLoading);
            //editCommand = new DelegateCommand(() => IsEditing = true, () => !IsEditing && !IsLoading);
            toggleEditCommand = new DelegateCommand(ToggleEdit, () => !IsEditingItem && !IsLoading);
            //saveCommand = new DelegateCommand<object>(Save);
            findCommand = new DelegateCommand<object>(Find);
        }

        private void ItemsView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IEditableCollectionView.IsEditingItem)
                || e.PropertyName == nameof(IEditableCollectionView.IsAddingNew))
            {
                OnPropertyChanged(nameof(IsEditingItem));
                CommitCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
                ReloadCommand.RaiseCanExecuteChanged();
                ToggleEditCommand.RaiseCanExecuteChanged();
            }
        }

        /*
        private void Save(object item)
        {
            var viewModel = item as TViewModel;

            if (viewModel != null && viewModel.IsChanged(context) && !viewModel.HasErrors)
            {
                viewModel.Add(context);
                //IsSaving = true;

                try
                {
                    context.SaveChanges();
                    //await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    MessageBox.Show(ex.GetType().ToString() + ": " + ex.Message + "\n"
                        + ex.InnerException?.GetType().ToString() + ": " + ex.InnerException?.Message + "\n"
                        + ex.InnerException?.InnerException?.GetType().ToString() + ": " + ex.InnerException?.InnerException?.Message);
                }

                //IsSaving = false;
            }
        }
        */

        private async Task LoadAsync()
        {
            IsLoading = true;
            var entities = await query(context).ToListAsync();
            IsLoading = false;

            foreach (var entity in entities)
            {
                items.Add(wrap(entity));
            }
        }

        private async Task ReloadAsync()
        {
            items.Clear();
            context.Dispose();
            context = new DokkaszEntities();
            await LoadAsync();
        }

        private void ToggleEdit()
        {
            IsInEditMode = !IsInEditMode;
        }

        private void CommitEdit()
        {
            if (ItemsView.IsEditingItem && !((TViewModel)ItemsView.CurrentEditItem).HasErrors)
            {
                ItemsView.CommitEdit();
            }
            else if (ItemsView.IsAddingNew && !((TViewModel)ItemsView.CurrentAddItem).HasErrors)
            {
                ItemsView.CommitNew();
            }
        }

        private void CancelEdit()
        {
            if (ItemsView.IsEditingItem)
            {
                ItemsView.CancelEdit();
            }
            else if (ItemsView.IsAddingNew)
            {
                ItemsView.CancelNew();
            }
        }

        public void Find(object value)
        {
            var stringValue = value.ToString();

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return;
            }

            var property = (PropertyAdapter<TViewModel>)propertiesView.CurrentItem;
            var item = ItemsView.OfType<TViewModel>().FirstOrDefault(i => property.GetValue(i).ToString().StartsWith(stringValue,
                StringComparison.CurrentCultureIgnoreCase));

            if (item != null)
            {
                ItemsView.MoveCurrentTo(item);
            }
        }

        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TViewModel item in e.NewItems)
                {
                    item.Context = context;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (TViewModel item in e.OldItems)
                {
                    item.Delete();
                    //item.Delete(context);
                }

                //context.SaveChanges();
                // await context.SaveChangesAsync();
            }
        }
    }
}
