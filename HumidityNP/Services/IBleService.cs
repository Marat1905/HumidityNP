using System;
using System.Threading.Tasks;
using HumidityNP.Models;

namespace HumidityNP.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с BLE-влагомером Delmhorst X0 Series.
    /// Обеспечивает автоматическое подключение, получение данных и переподключение.
    /// </summary>
    public interface IBleService : IDisposable
    {
        /// <summary>
        /// Событие, возникающее при получении новых данных от влагомера.
        /// </summary>
        event Action<ParsedHumidityData> OnDataReceived;

        /// <summary>
        /// Событие, возникающее при изменении статуса подключения.
        /// Передаёт строковое описание текущего состояния.
        /// </summary>
        event Action<string> OnStatusChanged;

        /// <summary>
        /// Возвращает true, если установлено активное BLE-соединение с устройством.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Запускает процесс автоматического поиска ближайшего устройства
        /// (по RSSI) и подключения к нему. При потере связи автоматически
        /// переподключается (пока не будет вызван DisconnectAsync).
        /// </summary>
        Task StartAutoConnectAsync();

        /// <summary>
        /// Отключает текущее устройство и останавливает автоматическое
        /// переподключение.
        /// </summary>
        Task DisconnectAsync();
    }
}