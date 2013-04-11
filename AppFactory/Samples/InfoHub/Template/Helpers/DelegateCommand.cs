using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoHub.Helpers
{
    public class DelegateCommand : System.Windows.Input.ICommand
    {
        private readonly Action m_Execute;
        private readonly Func<bool> m_CanExecute;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute)
            : this(execute, () => true) { /* empty */ }

        public DelegateCommand(Action execute, Func<bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            m_Execute = execute;
            m_CanExecute = canexecute;
        }

        public bool CanExecute(object p)
        {
            return m_CanExecute == null ? true : m_CanExecute();
        }

        public void Execute(object p)
        {
            if (CanExecute(null))
                m_Execute();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand<T> : System.Windows.Input.ICommand
    {
        private readonly Action<T> m_Execute;
        private readonly Func<T, bool> m_CanExecute;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute)
            : this(execute, (x) => true) { /* empty */ }

        public DelegateCommand(Action<T> execute, Func<T, bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            m_Execute = execute;
            m_CanExecute = canexecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object p)
        {
            try
            {
                var _Value = (T)Convert.ChangeType(p, typeof(T));
                return m_CanExecute == null ? true : m_CanExecute(_Value);
            }
            catch
            {
                Debugger.Break();
                return false;
            }
        }

        public void Execute(object p)
        {
            if (CanExecute(p))
                try
                {
                    var _Value = (T)Convert.ChangeType(p, typeof(T));
                    m_Execute(_Value);
                }
                catch { Debugger.Break(); }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }

}
