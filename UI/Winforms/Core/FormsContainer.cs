using CoreLib.Extensions;
using CoreLib.UI.Winforms.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLib.UI.Winforms.Core {
    public class FormsDiContainer : IFormsDiContainer {

        Dictionary<Type, Form> _formsContainer;
        IServiceProvider _serviceProvider;

        public FormsDiContainer(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _formsContainer = new Dictionary<Type, Form>();
        }

        public Form Get<TForm>() {
            Form formInstance = null;
            Type typeKey = typeof(TForm);

            if (_formsContainer.ContainsKey(typeKey)) {
                formInstance = _formsContainer[typeKey];
            } else {
                formInstance = ActivatorUtilities.CreateInstance(_serviceProvider, typeKey) as Form;
                formInstance.Closed += OnFormClosed;

                _formsContainer.Add(typeKey, formInstance);
            }

            return formInstance;
        }

        private void OnFormClosed(object sender, EventArgs e) {
            PerformCleanUp(sender as Form);
        }

        private void PerformCleanUp(Form form) {
            if (_formsContainer.IsNotNull()) {
                _formsContainer.Remove(form.GetType());
            }
        }

    }
}
