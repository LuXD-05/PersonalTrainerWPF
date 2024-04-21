using System.ComponentModel;

namespace PersonalTrainerApp.ViewModels
{
    /// <summary>
    /// Classe ViewModelbase che implementa l'INotifyPropertyChanged e da cui derivano il MainViewModel e gli altri
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        private string _title;
        private string _error;
        private double _height;
        private double _width;

        // Proprietà che contiene il titolo della view
        public string Title
        {
            get { return _title; }
            set 
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }


        // Proprietà che contiene una stringa di errore
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged(nameof(Error));
                }
            }
        }

        // Proprietà per adattare l'altezza della finestra a quella della view
        public double Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        // Proprietà per adattare la larghezza della finestra a quella della view
        public double Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

/*
 
Globals.xaml ------------------------------------------------------------------------------------------
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Style x:Key="Font" TargetType="Window">
        <Setter Property="FontSize" Value="14"/>
    </Style>
    
    <Style x:Key="btnAddSmall" TargetType="Button">
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Height" Value="auto" />
        <Setter Property="Width" Value="{Binding ActualHeight, RelativeSource={RelativeSource Self}}" />
        <Setter Property="Background" Value="Transparent" />
        <Setter Property="FontSize" Value="16" />

        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Grid>
                        <Grid.Style>
                            <Style TargetType="Grid">
                                <Style.Triggers>
                                    <Trigger Property="IsMouseOver" Value="true">
                                        <Setter Property="Background" Value="#FFB9B9B9"/>
                                    </Trigger>
                                    <Trigger Property="IsMouseOver" Value="false">
                                        <Setter Property="Background" Value="LightGray"/>
                                    </Trigger>
                                </Style.Triggers>
                            </Style>
                        </Grid.Style>
                        <TextBlock Text="{TemplateBinding Content}" 
                                   VerticalAlignment="Center" 
                                   HorizontalAlignment="Center"
                                   Margin="-1,-4,0,0" >
                        </TextBlock>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!--<Style TargetType="Image">
        <Style.Triggers>
            <DataTrigger Binding="{Binding Text,ElementName=tbk_titolo}" Value="magro morto">
                <Setter Property="Source" Value="../Resources/Immagine2.png"/>
            </DataTrigger>
            <DataTrigger Binding="{Binding Text,ElementName=tbk_titolo}" Value="smily">
                <Setter Property="Source" Value="../Resources/Immagine1.png"/>
            </DataTrigger>
            <Trigger Property="IsMouseOver"  Value="true">
                <Setter Property="Source" Value="../Resources/Immagine.png"/>
            </Trigger>
        </Style.Triggers>
    </Style>-->

</ResourceDictionary>
------------------------------------------------------------------------------------------------------------
*/
