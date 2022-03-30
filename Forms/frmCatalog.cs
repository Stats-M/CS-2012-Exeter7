using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Logs;
using StringHelper;

namespace Exeter7
{
    /// <summary>
    /// Класс пользовательского интерфейса для любых работ с каталогом запчастей. 
    /// </summary>
    public partial class frmCatalog : Form
    {

        #region Поля класса
        /// <summary>
        /// Экземпляр класса FileCatalog, отвечающий за работу с файлом каталога, 
        /// включая ввод-вывод и конвертирование сохраненных данных в разные форматы
        /// </summary>
        private FileCatalog _fileCatalog = null;
        #endregion

        #region Конструктор
        public frmCatalog()
        {
            InitializeComponent();
        }
        #endregion

        #region Методы-обработчики событий формы

        /// <summary>
        /// Метод, вызываемый при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmCatalog_Load(object sender, EventArgs e)
        {

            Program.Log.Write("Загрузка окна каталога запчастей");
        }
        #endregion

        #region Другие методы
        #endregion

    }
}
