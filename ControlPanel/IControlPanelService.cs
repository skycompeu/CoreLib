using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.ControlPanel {
    public interface IControlPanelService {
        List<ControlPanelItem> GetControlPanelItems(int iconSize = 48);
    }
}
