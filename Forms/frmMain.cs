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
    public partial class frmMain : Form
    {

        #region Поля класса

        /// <summary>
        /// Экземпляр класса frmCatalog, отвечающий за отображение окна и работу интерфейса каталога
        /// </summary>
        private frmCatalog _catalogWindow = null;

        /// <summary>
        /// Имя файла каталога. По-умолчанию берем из настроек, в дальнейшем пользователь может 
        /// указать иное имя и/или расположение файла.
        /// </summary>
        private string _catalogFilename = AppSettings.fileCatalog;

        #endregion

        #region Конструктор
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Методы-обработчики событий формы
        /// <summary>
        /// Метод, вызываемый при загрузке главной формы приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// Метод вызывается при закрытии формы ДО самого закрытия. Также метод 
        /// определяет причины закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// Метод, вызываемый при каждой активации окна (получении фокуса). 
        /// Обновляем элементы интерфейса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Activated(object sender, EventArgs e)
        {

            Program.Log.Write("frmMain::frmMain_Activated - Обновляем текст меток-описаний");

            // Обновляем текст меток с дефолтного "Loading..." на актуальную информацию (открыт ли уже каталог, наименование файла каталога...)
            Program.Log.Write("Обновляем описание статуса каталога запчастей.");
            StringBuilder msg = new StringBuilder("Файл каталога запчастей ", 256);
            if (_catalogWindow != null)
            {
                msg.Append("открыт. ");
            }
            else
            {
                msg.Append("не открыт. ");
            }
            msg.Append("Имя файла каталога:\r\n");
            msg.Append(_catalogFilename);
            lblCatalog.Text = msg.ToString();
        }
        
        /// <summary>
        /// Метод обрабатывает нажатие кнопки "Выход" любым способом (мышью, клавиатурой)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            SaveAll(true);  // Сохраняем все без вопросов
            this.Close();
        }

        /// <summary>
        /// Метод обрабатывает нажатие кнопки отрытия каталога запчастей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCatalog_Click(object sender, EventArgs e)
        {

            Program.Log.Write("frmMain::btnCatalog_Click - Вызов окна каталога запчастей");

            if (_catalogWindow == null)
            {
                Program.Log.Write("frmMain::btnCatalog_Click - Окно не существует, создаем");
                _catalogWindow = new frmCatalog();
                _catalogWindow.Show();
            }
            else
            {
                _catalogWindow.Activate();
            }

        }
        #endregion

        #region Другие методы
        /// <summary>
        /// Главный метод, производящий сохранение всех изменений
        /// </summary>
        public static void SaveAll(bool doSilently)
        {
            // DEBUG Как нужно использовать флаги.
            //ATM_FamilyList test;
            //test = ATM_FamilyList.NCR_SelfServ | ATM_FamilyList.NCR_50xx;
            //DialogResult result = MessageBox.Show(test.ToString() + " hahaha!!! " + (int)test, "Инфа!", MessageBoxButtons.OK);
        }

        #endregion

    }
}
