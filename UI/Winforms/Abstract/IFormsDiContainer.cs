using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLib.UI.Winforms.Abstract {
    public interface IFormsDiContainer {
        Form Get<TForm>();
    }
}
