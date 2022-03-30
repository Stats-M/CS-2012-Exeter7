using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Exeter7
{
    /// <summary>
    /// Класс пользовательского интерфейса для выбора (с последующей передачей) 
    /// или демонстрации битовых флагов - свойств запчасти (запчастей).
    /// </summary>
    /// <remarks>Класс позволяет наглядно проводить изменения при помощи checkbox'ов</remarks>
    public partial class frmFilterPart : Form
    {
        public frmFilterPart()
        {
            InitializeComponent();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO write code here
        }
    }
}
