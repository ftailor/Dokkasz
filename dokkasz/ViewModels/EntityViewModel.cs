using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace dokkasz.ViewModels
{
    public abstract class EntityViewModel : INotifyPropertyChanged, IEditableObject, IDataErrorInfo
    {
        private bool isEditing;
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        public DbContext Context { get; set; }

        protected abstract object Entity { get; }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                if (!errors.ContainsKey(columnName))
                {
                    ValidateProperty(columnName);
                }

                return errors[columnName];
            }
        }

        public bool HasErrors
        {
            get
            {
                var propertyNames = from p in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                    where Attribute.IsDefined(p, typeof(ValidationAttribute)) && !errors.ContainsKey(p.Name)
                                    select p.Name;

                foreach (var propertyName in propertyNames)
                {
                    ValidateProperty(propertyName);
                }

                return errors.Any(e => e.Value != null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            ValidateProperty(propertyName);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ValidateProperty(string propertyName)
        {
            string oldError;
            errors.TryGetValue(propertyName, out oldError);

            var value = GetType().GetProperty(propertyName).GetValue(this);
            var context = new ValidationContext(this) { MemberName = propertyName };
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateProperty(value, context, results))
            {
                var error = results[0].ErrorMessage;

                if (!string.Equals(oldError, error, StringComparison.Ordinal))
                {
                    errors[propertyName] = error;
                }
            }
            else
            {
                errors[propertyName] = null;
            }
        }

        public void BeginEdit()
        {
            isEditing = true;
        }

        public void EndEdit()
        {
            if (isEditing && !HasErrors)
            {
                isEditing = false;
                var entry = Context.Entry(Entity);

                if (entry.State == EntityState.Detached)
                {
                    entry.State = EntityState.Added;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    Context.SaveChanges();
                }
            }
        }

        public void CancelEdit()
        {
            if (isEditing)
            {
                isEditing = false;
                var entry = Context.Entry(Entity);

                if (entry.State == EntityState.Modified)
                {
                    entry.Reload();
                    errors.Clear();
                }
            }
        }

        public void Delete()
        {
            var entry = Context.Entry(Entity);

            if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
            {
                entry.State = EntityState.Deleted;

                Context.SaveChanges();
            }
        }

        /*
        public void Add(DbContext context)
        {
            var entry = context.Entry(Entity);

            if (entry.State == EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
        }

        public void Delete(DbContext context)
        {
            var entry = context.Entry(Entity);

            if (entry.State == EntityState.Unchanged || entry.State == EntityState.Modified)
            {
                entry.State = EntityState.Deleted;
            }
            else if (entry.State == EntityState.Added)
            {
                entry.State = EntityState.Detached;
            }
        }

        public EntityKey GetEntityKey(DbContext context)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            return objectContext.ObjectStateManager.GetObjectStateEntry(Entity).EntityKey;
        }

        public bool IsChanged(DbContext context)
        {
            return context.Entry(Entity).State != EntityState.Unchanged;
        }
        */
    }

    /*
    public abstract class EntityViewModel
    {
        private Dictionary<string, List<string>> errors;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            var value = GetType().GetProperty(e.PropertyName).GetValue(this);
            var context = new ValidationContext(this) { MemberName = e.PropertyName };
            var results = new List<ValidationResult>();
            bool errorsChanged;

            if (!Validator.TryValidateProperty(value, context, results))
            {
                List<string> oldErrors;

                if (!errors.TryGetValue(e.PropertyName, out oldErrors))
                {
                    oldErrors = null;
                }

                var newErrors = results.OrderBy(r => r.ErrorMessage, StringComparer.Ordinal).Select(r => r.ErrorMessage);
                errorsChanged = !(oldErrors?.SequenceEqual(newErrors) ?? false);

                if (errorsChanged)
                {
                    errors[e.PropertyName] = newErrors.ToList();
                }
            }
            else
            {
                errorsChanged = errors.Remove(e.PropertyName);
            }

            if (errorsChanged && ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(e.PropertyName));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                return null;
            }

            List<string> propertyErrors;
            return errors.TryGetValue(propertyName, out propertyErrors) ? propertyErrors : null;
        }

        protected EntityViewModel()
        {
            errors = new Dictionary<string, List<string>>();
            PropertyChanged += OnPropertyChanged;
        }

        public virtual string this[string columnName]
        {
            get
            {
                var value = GetType().GetProperty(columnName).GetValue(this);
                var context = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();
                return !Validator.TryValidateProperty(value, context, results) ? results.First().ErrorMessage : null;
            }
        }
    }
    */
}
