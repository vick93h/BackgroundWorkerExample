using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundWorkerExample
{
    public class Singleton
    {
        private DatiScambio _dati;
        private static Singleton _currentState;
        public DatiScambio DatiScambio => _dati;
        private Singleton()
        {
            _dati = new DatiScambio();
        }
        

        public static Singleton CurrentState
        {
            get
            {
                if (_currentState == null)
                    _currentState = new Singleton();

                return _currentState;
            }
        }
    }
}
