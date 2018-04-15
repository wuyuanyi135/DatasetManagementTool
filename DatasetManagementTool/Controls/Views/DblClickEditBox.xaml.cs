using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatasetManagementTool.Controls.Views
{
    /// <summary>
    /// Interaction logic for DblClickEditBox.xaml
    /// </summary>
    public partial class DblClickEditBox : TextBox
    {
        public DblClickEditBox()
        {
            InitializeComponent();
            ContextMenu = null;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
