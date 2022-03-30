﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exeter7
{
    // НЕ ИСПОЛЬЗУЕТСЯ С ВЕРСИИ СБОРКИ 1.0.1.6 (ВЕРСИЯ ФАЙЛА 1.0.1.15)
    // Объявления методов интерфейса перенесены в базовый класс и сделаны protected virtual.
    // 16 марта 2012

    /// <summary>
    /// Интерфейс, реализующий совместно с классом BinaryStreamMaster единый для всех классов 
    /// файлов данных механизм работы с файлом и мастер-записью. Классы-наследники BinaryStreamMaster 
    /// обязаны также реализовать интерфейс IBinaryStreamMaster (см. remarks для подробной информации).</summary>
    /// 
    /// <remarks>Идея разделения реализации работы с файлом и мастер-записью на базовый класс и интерфейс 
    /// основывается на том, что часть методов является общей для всех конечных классов (например, 
    /// методы открытия файла), а часть - при одинаковой сигнатуре вызова имеет в каждом конечном 
    /// классе уникальную реализацию. К тому же, при реализации интерфейса невозможно реализовать 
    /// private/protected методы (интерфейс требует чтобы все методы были public), что не очень 
    /// хорошо, например, для методов открытия/закрытия файла; их вызовы должны быть скрыты от вызова 
    /// извне.
    /// В итоге, общие методы реализуются в базовом классе BinaryStreamMaster и объявляются как protected, 
    /// а остальные обязательные методы, имеющие уникальную для каждого конечного класса реализацию, 
    /// объявляются через интерфейс IBinaryStreamMaster. Единственный недостаток (несущественный в 
    /// рамках данного проекта) - методы реализации интерфейса будут иметь доступ public.
    /// 
    /// Интерфейс содержит объявления методов, которые должны быть реализованы в конечных классах в обязательном 
    /// порядке. При этом конкретная реализация их является уникальной для каждого класса. Например, методы 
    /// по дешифровке мастер записи, очевидно, уникальны в каждом классе.</remarks>
    interface IBinaryStreamMaster
    {

        /// <summary>
        /// Метод, производящий расшифровку служебной информации из начала файла (мастер-запись). 
        /// При расшифровке необходимо учитывать версию данных, записанных в файл.
        /// </summary>
        bool DecodeMasterFrame();

        /// <summary>
        /// Метод, производящий кодирование служебной информации в байтовый массив для его 
        /// последующей записи в начало файла данных (мастер-запись).
        /// </summary>
        /// <param name="VersionData">Версия данных, в которой следуют производить кодировку мастер-записи</param>
        bool EncodeMasterFrame(int VersionData);

        /// <summary>
        /// Метод, производящий перевод старой версии мастер-записи в новую версию (в текущую версию кода).
        /// </summary>
        /// <returns>true, если конверсия успешно произведена</returns>
        bool UpgradeMasterFrame();
    }
}