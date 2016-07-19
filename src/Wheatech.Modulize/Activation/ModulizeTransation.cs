using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Wheatech.Modulize
{
    internal class ModulizeTransation : IDisposable
    {
        private bool _completed;
        private List<Action> _commitActions = new List<Action>();
        private List<Action> _rollbackActions = new List<Action>();
        private TransactionScope _transactionScope = new TransactionScope(TransactionScopeOption.Required);

        public void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            if (_completed)
            {
                try
                {
                    foreach (var commitAction in _commitActions)
                    {
                        commitAction();
                    }
                    _transactionScope.Complete();
                }
                catch (Exception)
                {
                    Rollback();
                    throw;
                }
                finally
                {
                    Clear();
                }
            }
            else
            {
                Rollback();
                Clear();
            }
        }

        private void Rollback()
        {
            foreach (var rollbackAction in ((IEnumerable<Action>)_rollbackActions).Reverse())
            {
                rollbackAction();
            }
        }

        private void Clear()
        {
            _commitActions.Clear();
            _commitActions = null;
            _rollbackActions.Clear();
            _rollbackActions = null;
            _transactionScope.Dispose();
            _transactionScope = null;
        }

        public void Enlist(Action commitAction, Action rollbackAction)
        {
            if (commitAction != null)
            {
                _commitActions.Add(commitAction);
            }
            if (rollbackAction != null)
            {
                _rollbackActions.Add(rollbackAction);
            }
        }
    }
}
