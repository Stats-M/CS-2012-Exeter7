using System.Diagnostics;   // For access to Debug and Trace classes

namespace Exeter7.Properties {
    
    
    /// <summary>
    /// Этот класс позволяет обрабатывать определенные события в классе параметров:
    ///  Событие SettingChanging возникает перед изменением значения параметра.
    ///  Событие PropertyChanged возникает после изменения значения параметра.
    ///  Событие SettingsLoaded возникает после загрузки значений параметров.
    ///  Событие SettingsSaving возникает перед сохранением значений параметров.
    /// </summary>
    internal sealed partial class Settings {
        
        /// <summary>
        /// Конструктор класса параметров, определяет реализованные обработчики событий 
        /// и связывает их с самими событиями
        /// </summary>
        public Settings() {
            // // Для добавления обработчиков событий для сохранения и изменения параметров раскомментируйте приведенные ниже строки:
            //
            this.SettingChanging += this.SettingChangingEventHandler;
            //
            this.SettingsSaving += this.SettingsSavingEventHandler;
            //
            this.SettingsLoaded += this.SettingsLoadedEventHandler;
            //
            this.PropertyChanged += this.PropertyChangedEventHandler;
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // TODO Добавьте здесь код для обработки события SettingChangingEvent.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("****************** Settings has been saved!!!");
            Program.Log.Write("Начинаем сохранять настройки программы");
            
            // TODO Добавьте здесь код для обработки события SettingsSaving.
        }

        private void SettingsLoadedEventHandler(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            // Сюда программа заходит 1 раз, еще до инициализации лог-файла, поэтому в лог не пишем ничего, 
            // иначе будет исключение System.NullReferenceException; только отладочное сообщение.
            Debug.WriteLine("****************** Settings has been loaded!!! Can use them now");

            // DEBUG Debug.Assert(false, "****************** Settings has been loaded!!! Can use them now");

            // TODO Добавьте здесь код для обработки события SettingsLoaded.
        }

        private void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO Добавьте здесь код для обработки события PropertyChanged.
        }
    }
}
