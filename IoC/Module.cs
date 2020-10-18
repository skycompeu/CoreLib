using CoreLib.Di;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.DependencyInjection {
    public abstract class Module {
        public abstract void RegisterServices(Container Container);
    }
}
