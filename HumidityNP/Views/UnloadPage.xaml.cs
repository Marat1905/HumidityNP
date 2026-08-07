using HumidityNP.ViewModels;

namespace HumidityNP.Views;

/// <summary>
/// Страница для ввода/редактирования данных разгрузки машины.
/// Принимает параметры: vehicleId (для новой записи) или localId (для редактирования существующей).
/// Если передан localId, загружает существующую запись и позволяет её редактировать.
/// </summary>
[QueryProperty(nameof(VehicleId), "vehicleId")]
[QueryProperty(nameof(LocalId), "localId")]
public partial class UnloadPage : ContentPage
{
    private readonly UnloadViewModel _viewModel;

    public UnloadPage(UnloadViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// Идентификатор машины (для новой записи).
    /// </summary>
    public string VehicleId
    {
        set => _viewModel.VehicleId = value;
    }

    /// <summary>
    /// Локальный ID разгрузки (для редактирования существующей записи).
    /// Если значение > 0, загружаем запись для редактирования.
    /// </summary>
    public string LocalId
    {
        set
        {
            if (int.TryParse(value, out int id) && id > 0)
            {
                _viewModel.LoadForEditAsync(id);
            }
        }
    }
}