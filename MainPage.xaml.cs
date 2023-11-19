using CommunityToolkit.Maui.Views;
using Ejercicio2_4.Models;
using Ejercicio2_4.Vistas;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ejercicio2_4
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        string videoPath;

        public MainPage()
        {
            InitializeComponent();

        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {
            var videoOptions = new MediaPickerOptions
            {
                Title = "Capturar video"
            };

            var video = await MediaPicker.CaptureVideoAsync(videoOptions);

            if (video != null)
            {
                using (var stream = await video.OpenReadAsync())
                {
                    videoPath = GetVideoPath(stream);

                    img.Source = MediaSource.FromUri(new Uri(videoPath));
                }
            }
        }

        private async void btnSQlite_Clicked(object sender, EventArgs e)
        {
            // Verificar que se haya capturado un video
            if (string.IsNullOrEmpty(videoPath))
            {
                await DisplayAlert("Atencion", "Por favor, capture un video antes de guardar.", "Ok");
                return;
            }

            // Verificar que se hayan ingresado nombres y descripción
            if (string.IsNullOrWhiteSpace(txtnombre.Text) || string.IsNullOrWhiteSpace(txtdescripcion.Text))
            {
                await DisplayAlert("Atencion", "Por favor, ingrese nombres y descripción antes de guardar.", "Ok");
                return;
            }

            var empleado = new Empleados
            {
                nombres = txtnombre.Text,
                descripcion = txtdescripcion.Text,
                imagen = await File.ReadAllBytesAsync(videoPath)
            };

            var resultado = await App.BaseDatos.EmpleSave(empleado);

            if (resultado != 0)
            {
                await DisplayAlert("Atencion", "El Video fue ingresado correctamente!!!", "Ok");

                // Limpiar los campos y la imagen después de guardar
                txtnombre.Text = string.Empty;
                txtdescripcion.Text = string.Empty;
                videoPath = string.Empty;
                img.Source = null;
            }
            else
            {
                await DisplayAlert("Atencion", "Upps ha ocurrido un error inesperado", "Ok");
            }

            await Navigation.PopAsync();
        }

        private string GetVideoPath(Stream stream)
        {
#if __ANDROID__
            var publicDirectoryPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            var videoPath = Path.Combine(publicDirectoryPath, "video.mp4");

            using (var fileStream = File.Create(videoPath))
            {
                stream.CopyTo(fileStream);
            }

            return videoPath;
#else
            return string.Empty;
#endif
        }

        private async void btnLista_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaEmpleados());
        }


    }
}
