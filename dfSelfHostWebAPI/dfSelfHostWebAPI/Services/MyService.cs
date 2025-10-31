using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfSelfHostWebAPI.Services
{
    public interface IMyService
    {
        string Greet( string name );
    }

    public class MyService : IMyService
    {
        public string Greet( string name ) => $"Hello {name}, from SelfHost!";
    }
}
