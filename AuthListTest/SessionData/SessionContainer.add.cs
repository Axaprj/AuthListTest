using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public partial class SessionsContainer
    {
        public void OnContextCreated()
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
    }
}
