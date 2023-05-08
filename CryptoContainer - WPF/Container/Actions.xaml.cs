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

namespace ILyaContainer
{
    /// <summary>
    /// Interaction logic for Actions.xaml
    /// </summary>
    public partial class Actions : Page
    {
        private void SetComponents(string cont)
        {
            textbox_content.Text = cont;
        }
        public Actions(string str)
        {
            InitializeComponent();
            SetComponents(str);
        }

        private void button_OK_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);

        }
    }
}
