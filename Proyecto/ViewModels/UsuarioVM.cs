namespace Proyecto.ViewModels
{

    //Esta clase se crea como buena practica para trabajar con nuestro modelo de usuario sin utilizar el modelo base si no como una extencion 
    public class UsuarioVM
    {

        public string NombreCompleto { get; set; }

        public string Correo { get; set; }

        public string clave { get; set; }

        public string ConfirmarClave { get; set; }

    }
}
