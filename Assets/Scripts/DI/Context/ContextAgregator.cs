using System;
using System.Collections.Generic;

namespace DI
{
    public class ContextAgregator
    {
        private List<IReadOnlyContext> _contexts = new List<IReadOnlyContext>();
        private Dictionary<Type, ISource> _combinedContext = new Dictionary<Type, ISource>();
        private IErrorProvider _errorProvider;

        public ContextAgregator(IErrorProvider errorProvider) => _errorProvider = errorProvider;

        public void Add(IReadOnlyContext context)
        {
            if(_contexts.Contains(context))
            {
                _errorProvider.Throw(new InvalidOperationException("Try to add already added context"));
                return;
            }

            context.SourceAdded += AddToCombined;
            context.SourceRemoved += RemoveFromCombined;
            AddToCombined(context);
            _contexts.Add(context);
        }

        public void Remove(IReadOnlyContext context)
        {
            if (_contexts.Contains(context))
            {
                _errorProvider.Throw(new NullReferenceException("Try to remove non added context"));
                return;
            }

            context.SourceAdded -= AddToCombined;
            context.SourceRemoved -= RemoveFromCombined;
            RemoveFromCombined(context);
            _contexts.Remove(context);
        }

        public object GetObject(Type type)
        {
            if (_combinedContext.TryGetValue(type, out ISource source))
                return source.Value;

            _errorProvider.Throw(new NullReferenceException($"Try to get non existing source typeof {type}"));
            return GetDefault(type);
        }

        private void AddToCombined(IReadOnlyContext context)
        {
            foreach (KeyValuePair<Type, ISource> pair in context.GetSources())
                AddToCombined(pair.Key, pair.Value);
        }

        private void AddToCombined(Type type, ISource source)
        {
            if(_combinedContext.ContainsKey(type))
            {
                _errorProvider.Throw(new InvalidCastException($"Context collision, source typeof {type} already exist in some context"));
                return;
            }

            _combinedContext.Add(type, source);
        }

        private void RemoveFromCombined(IReadOnlyContext context)
        {
            foreach (KeyValuePair<Type, ISource> pair in context.GetSources())
                RemoveFromCombined(pair.Key, pair.Value);

        }

        private void RemoveFromCombined(Type type, ISource source)
        {
            if (_combinedContext.ContainsKey(type) == false)
            {
                _errorProvider.Throw(new NullReferenceException($"Context collision, source typeof {type} dont exists in combined context"));
                return;
            }

            _combinedContext.Remove(type);
        }

        private static object GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}