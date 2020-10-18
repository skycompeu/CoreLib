using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.UI.Winforms.Abstract {
    public interface IForm {
        Guid FormId { get; set; }
        string FormName { get; set; }
    }
}
